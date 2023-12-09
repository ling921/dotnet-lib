using Ling.EntityFrameworkCore.Audit.Extensions;
using Ling.EntityFrameworkCore.Audit.Internal.Extensions;
using Ling.EntityFrameworkCore.Audit.Internal.Models;
using Ling.EntityFrameworkCore.Audit.Models;
using Ling.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Ling.EntityFrameworkCore.Audit.Internal;

internal sealed class AuditInterceptor<TUserKey> : SaveChangesInterceptor
{
    private static readonly ConcurrentDictionary<DbContextId, List<AuditEntry>> s_contextEntries = new();

    /// <inheritdoc/>
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        InternalSavingChanges(eventData);
        return base.SavingChanges(eventData, result);
    }

    /// <inheritdoc/>
    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        var count = InternalSavedChanges(eventData);
        if (count > 0)
        {
            eventData.Context!.SaveChanges();
        }
        return result;
    }

    /// <inheritdoc/>
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        InternalSavingChanges(eventData);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// <inheritdoc/>
    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result, CancellationToken
        cancellationToken = default)
    {
        var count = InternalSavedChanges(eventData);
        if (count > 0)
        {
            await eventData.Context!.SaveChangesAsync(cancellationToken);
        }
        return result;
    }

    internal void InternalSavingChanges(DbContextEventData eventData)
    {
        var context = eventData.Context;

        if (context is null) return;

        var userProvider = context.GetService<IAuditUserProvider<TUserKey>>();
        var logger = context.GetService<ILoggerFactory>().CreateLogger(GetType());
        var options = context.GetAuditOptions();
        var entries = new List<AuditEntry>();
        var now = DateTimeOffset.Now;
        var isUserIdDefaultValue = EqualityComparer<TUserKey>.Default.Equals(userProvider.Id, default);

        foreach (var entityEntry in context.ChangeTracker.Entries())
        {
            var entityType = entityEntry.Metadata.ClrType;
            var metadata = entityEntry.Metadata.GetAuditMetadata();
            var eventType = EventType.None;
            var userId = isUserIdDefaultValue ? null : userProvider.Id.ConvertToTargetType(metadata.UserKeyType);

            if (typeof(TUserKey).IsSameTypeIgnoreNullableTo(metadata.UserKeyType))
            {
                logger.LogError(
                    "The type of 'TUserKey' configured is '{UserKeyType}', but entity's user key type is '{entityType}'.",
                    typeof(TUserKey),
                    metadata.UserKeyType);
                throw new InvalidOperationException($"The type of 'TUserKey' configured is not match entity [{entityType}].");
            }

            switch (entityEntry.State)
            {
                case EntityState.Deleted:
                    if (metadata.HasDeletionTime)
                    {
                        entityEntry.Property(Constants.DeletionTime).CurrentValue = now;
                    }

                    if (metadata.HasDeleterId)
                    {
                        if (!options.AllowAnonymousDelete &&
                            !metadata.IsAllowedAnonymousOperate(AuditOperate.Delete) &&
                            isUserIdDefaultValue)
                        {
                            logger.LogError("Not allowed to delete entity '{entityType}' with anonymous user.", entityType);
                            throw new InvalidOperationException($"Anonymous deletion of '{entityType.GetFriendlyName()}' is not allowed.");
                        }
                        entityEntry.Property(Constants.DeleterId).CurrentValue = userId;
                    }

                    if (metadata.HasIsDeleted)
                    {
                        entityEntry.Property(Constants.IsDeleted).CurrentValue = true;
                        entityEntry.State = EntityState.Modified;
                        eventType = EventType.SoftDelete;
                    }
                    else
                    {
                        eventType = EventType.Delete;
                    }
                    break;

                case EntityState.Modified:
                    if (metadata.HasLastModificationTime)
                    {
                        entityEntry.Property(Constants.LastModificationTime).CurrentValue = now;
                    }
                    if (metadata.HasLastModifierId)
                    {
                        if (!options.AllowAnonymousModify &&
                            !metadata.IsAllowedAnonymousOperate(AuditOperate.Update) &&
                            isUserIdDefaultValue)
                        {
                            logger.LogError("Not allowed to modify entity '{entityType}' with anonymous user.", entityType);
                            throw new InvalidOperationException($"Anonymous modification of {entityType.GetFriendlyName()} is not allowed.");
                        }

                        entityEntry.Property(Constants.LastModifierId).CurrentValue = userId;
                    }
                    eventType = GetModifiedType(entityEntry, metadata);
                    break;

                case EntityState.Added:
                    if (metadata.HasCreationTime)
                    {
                        entityEntry.Property(Constants.CreationTime).CurrentValue = now;
                    }
                    if (metadata.HasCreatorId)
                    {
                        if (!options.AllowAnonymousCreate &&
                            !metadata.IsAllowedAnonymousOperate(AuditOperate.Create) &&
                            isUserIdDefaultValue)
                        {
                            logger.LogError("Not allowed to create entity '{entityType}' with anonymous user.", entityType);
                            throw new InvalidOperationException($"Anonymous creation of {entityType.GetFriendlyName()} is not allowed.");
                        }
                        entityEntry.Property(Constants.CreatorId).CurrentValue = userId;
                    }
                    eventType = EventType.Create;
                    break;

                case EntityState.Detached:
                case EntityState.Unchanged:
                default:
                    break;
            }

            if (eventType is not EventType.None && TryGetAuditEntry(entityEntry, out var auditEntry))
            {
                auditEntry.EventType = eventType;
                entries.Add(auditEntry);
            }
        }

        if (!AppContext.TryGetSwitch(AuditDefaults.DisabledSwitchKey, out var disabled) || !disabled)
        {
            s_contextEntries.TryAdd(eventData.Context!.ContextId, entries);
        }
    }

    internal int InternalSavedChanges(SaveChangesCompletedEventData eventData)
    {
        var context = eventData.Context;

        if (context is null || (AppContext.TryGetSwitch(AuditDefaults.DisabledSwitchKey, out var disabled) && disabled)) return 0;

        var logger = context.GetService<ILoggerFactory>().CreateLogger(GetType());
        if (!s_contextEntries.TryRemove(context.ContextId, out var entryInfo))
        {
            logger.LogWarning("Unable to get entry information when saved changes.");
            throw new InvalidOperationException("Unable to get entry information before saving changes.");
        }

        var userProvider = context.GetService<IAuditUserProvider<TUserKey>>();
        var logs = entryInfo
            .ConvertAll(i => new AuditEntityLog<TUserKey>
            {
                Schema = i.Schema,
                Table = i.Table,
                PrimaryKey = i.PrimaryKey,
                EntityType = i.EntityType,
                EventType = i.EventType,
                EventTime = DateTimeOffset.Now,
                OperatorId = userProvider.Id,
                OperatorName = userProvider.Name,
                Details = i.Properties.ConvertAll(ii => new AuditFieldLog
                {
                    PropertyName = i.EntityType + '.' + ii.PropertyName,
                    OriginalValue = GetStringValue(ii.OriginalValue),
                    NewValue = GetStringValue(ii.NewValue),
                }),
            });

        var count = logs.Sum(i => i.Details.Count + 1);
        if (count > 0)
        {
            context.AddRange(logs);

            logger.LogInformation(
                "Add {Count} audit logs to {DbContextType}[{ContextId}].",
                count,
                context.GetType().GetFriendlyName(),
                context.ContextId);
        }
        return count;
    }

    private static bool TryGetAuditEntry(EntityEntry entityEntry, [NotNullWhen(true)] out AuditEntry? auditEntry)
    {
        var entityInclude = entityEntry.Metadata.GetAuditInclude();
        if (!entityInclude)
        {
            auditEntry = null;
            return false;
        }

        auditEntry = new AuditEntry(entityEntry);

        foreach (var propertyEntry in entityEntry.Properties)
        {
            var propertyInclude = propertyEntry.Metadata.GetAuditInclude();
            if (propertyInclude && !Constants.PropertyNames.Contains(propertyEntry.Metadata.Name))
            {
                if (entityEntry.State is not EntityState.Added && Equals(propertyEntry.OriginalValue, propertyEntry.CurrentValue))
                {
                    continue;
                }

                auditEntry.Properties.Add(new AuditPropertyEntry
                {
                    PropertyName = propertyEntry.Metadata.Name,
                    OriginalValue = entityEntry.State == EntityState.Added ? null : propertyEntry.OriginalValue,
                    NewValue = propertyEntry.CurrentValue
                });
            }
        }

        return true;
    }

    private static EventType GetModifiedType(EntityEntry entityEntry, AuditMetadata metadata)
    {
        if (metadata.HasIsDeleted)
        {
            var originalValue = (bool)entityEntry.Property(Constants.IsDeleted).OriginalValue!;
            var newValue = (bool)entityEntry.Property(Constants.IsDeleted).CurrentValue!;
            if (originalValue != newValue)
            {
                return newValue ? EventType.SoftDelete : EventType.Recovery;
            }
        }
        return EventType.Modify;
    }

    private static string? GetStringValue(object? value)
    {
        if (value == null) return null;

        try
        {
            var converter = TypeDescriptor.GetConverter(value);
            if (converter.CanConvertTo(typeof(string)))
            {
                return converter.ConvertToString(value);
            }
        }
        catch { }

        return JsonSerializer.Serialize(value);
    }
}
