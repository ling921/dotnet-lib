using Ling.EntityFrameworkCore.Audit.Extensions;
using Ling.EntityFrameworkCore.Audit.Internal.Models;
using Ling.EntityFrameworkCore.Audit.Models;
using Ling.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Ling.EntityFrameworkCore.Audit.Internal;

internal sealed class AuditInterceptor : SaveChangesInterceptor
{
    private static readonly ConcurrentDictionary<DbContextId, List<AuditEntry>> s_contextEntries = new();

    /// <inheritdoc/>
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context is not null)
        {
            var userProvider = context.GetService<IAuditUserProvider>();
            var options = context.GetService<IOptionsSnapshot<AuditOptions>>().Value;
            var entries = AuditEntries(context, options, userProvider).ToList();

            if (AppContext.TryGetSwitch(AuditConstants.DisabledSwitchKey, out var disabled) || !disabled)
            {
                s_contextEntries.TryAdd(context.ContextId, entries);
            }
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// <inheritdoc/>
    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result, CancellationToken
        cancellationToken = default)
    {
        var context = eventData.Context;
        AppContext.TryGetSwitch(AuditConstants.DisabledSwitchKey, out var disabled);
        if (context is not null && !disabled)
        {
            var logger = context.GetService<ILoggerFactory>().CreateLogger(context.GetType());
            if (!s_contextEntries.TryRemove(context.ContextId, out var entryInfo))
            {
                throw new InvalidOperationException("Unable to get entry information before saving changes.");
            }

            var userProvider = context.GetService<IAuditUserProvider>();
            var logs = entryInfo
                .ConvertAll(i => new AuditLog
                {
                    Schema = i.Schema,
                    Table = i.Table,
                    PrimaryKey = i.PrimaryKey,
                    EntityType = i.EntityType,
                    EventType = i.EventType,
                    EventTime = DateTimeOffset.Now,
                    OperatorId = userProvider.Identity,
                    OperatorName = userProvider.Name,
                    Details = i.Properties.ConvertAll(ii => new AuditLogDetail
                    {
                        PropertyName = i.EntityType + '.' + ii.PropertyName,
                        OriginalValue = GetStringValue(ii.OriginalValue),
                        NewValue = GetStringValue(ii.NewValue),
                    }),
                });

            if (logs.Count > 0)
            {
                context.AddRange(logs);
                context.SaveChanges();

                logger.LogInformation("Successfully audited entity changes.");
            }
        }

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    private static IEnumerable<AuditEntry> AuditEntries(DbContext context, AuditOptions options, IAuditUserProvider userProvider)
    {
        var now = DateTimeOffset.Now;
        var isDefault = string.IsNullOrWhiteSpace(userProvider.Identity);
        foreach (var entityEntry in context.ChangeTracker.Entries())
        {
            var entityType = entityEntry.Metadata.ClrType;
            var metadata = entityEntry.Metadata.GetAuditMetadata();
            var eventType = EventType.None;
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
                            !metadata.IsAllowed(AuditOperate.Delete) &&
                            isDefault)
                        {
                            throw new InvalidOperationException($"Anonymous deletion of {entityType.GetFriendlyName()} is not allowed.");
                        }
                        entityEntry.Property(Constants.DeleterId).CurrentValue = userProvider.GeIdentityOfType(metadata.UserKeyType!);
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
                            !metadata.IsAllowed(AuditOperate.Update) &&
                            isDefault)
                        {
                            throw new InvalidOperationException($"Anonymous modification of {entityType.GetFriendlyName()} is not allowed.");
                        }

                        entityEntry.Property(Constants.LastModifierId).CurrentValue = userProvider.GeIdentityOfType(metadata.UserKeyType!);
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
                            !metadata.IsAllowed(AuditOperate.Create) &&
                            isDefault)
                        {
                            throw new InvalidOperationException($"Anonymous creation of {entityType.GetFriendlyName()} is not allowed.");
                        }
                        entityEntry.Property(Constants.CreatorId).CurrentValue = userProvider.GeIdentityOfType(metadata.UserKeyType!);
                    }
                    eventType = EventType.Create;
                    break;

                case EntityState.Detached:
                case EntityState.Unchanged:
                default:
                    break;
            }

            if (eventType != EventType.None && TryGetAuditEntry(entityEntry, out var auditEntry))
            {
                auditEntry.EventType = eventType;
                yield return auditEntry;
            }
        }
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
                if (entityEntry.State != EntityState.Added && Equals(propertyEntry.OriginalValue, propertyEntry.CurrentValue))
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
        if (value == null)
        {
            return null;
        }

        var type = value.GetType();
        type = Nullable.GetUnderlyingType(type) ?? type;

        return type.IsPrimitive || type == typeof(string)
            ? value.ToString()
            : JsonSerializer.Serialize(value);
    }
}
