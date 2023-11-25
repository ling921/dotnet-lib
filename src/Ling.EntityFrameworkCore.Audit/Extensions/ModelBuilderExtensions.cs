using Ling.EntityFrameworkCore.Audit;
using Ling.EntityFrameworkCore.Audit.Extensions;
using Ling.EntityFrameworkCore.Audit.Internal;
using Ling.EntityFrameworkCore.Audit.Internal.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Ling.EntityFrameworkCore.Extensions;

/// <summary>
/// Extension methods for building model.
/// </summary>
public static class ModelBuilderExtensions
{
    #region Public Methods

    /// <summary>
    /// Setup a global query filter for entities has "IsDeleted" property.
    /// </summary>
    /// <param name="builder">Entity model builder.</param>
    public static void SetupSoftDeleteQueryFilter(this ModelBuilder builder)
    {
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            var type = entityType.ClrType;

            if (entityType.HasProperty(Constants.IsDeleted))
            {
                var typeParameter = Expression.Parameter(type, "e");
                var propertyParameter = Expression.Property(typeParameter, Constants.IsDeleted);
                var lambda = Expression.Lambda(Expression.Not(propertyParameter), typeParameter);

                builder.Entity(type).HasQueryFilter(lambda);
            }
        }
    }

    /// <summary>
    /// Configures enable auditing to be applied to the table this entity is mapped to.
    /// </summary>
    /// <param name="entityTypeBuilder">The builder for the entity type being configured.</param>
    /// <param name="allowAnonymousOperate">
    /// Allowed anonymous operations to the table this entity is mapped to.
    /// </param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public static EntityTypeBuilder IsAuditable(this EntityTypeBuilder entityTypeBuilder, AuditOperate allowAnonymousOperate = AuditOperate.None)
    {
        ArgumentNullException.ThrowIfNull(entityTypeBuilder);

        entityTypeBuilder.Metadata.SetAnnotation(AuditAnnotationNames.Include, true);
        entityTypeBuilder.Metadata.SetAuditMetadata(new AuditMetadata { AllowAnonymousOperate = allowAnonymousOperate });

        return entityTypeBuilder;
    }

    /// <inheritdoc cref="IsAuditable(EntityTypeBuilder, AuditOperate)"/>
    /// <typeparam name="TEntity">The entity type being configured.</typeparam>
    public static EntityTypeBuilder<TEntity> IsAuditable<TEntity>(this EntityTypeBuilder<TEntity> entityTypeBuilder, AuditOperate allowAnonymousOperate = AuditOperate.None)
        where TEntity : class
        => (EntityTypeBuilder<TEntity>)((EntityTypeBuilder)entityTypeBuilder).IsAuditable(allowAnonymousOperate);

    /// <summary>
    /// Configures whether to apply auditing to the column.
    /// </summary>
    /// <param name="propertyBuilder">The builder for the property being configured.</param>
    /// <param name="enabled">Whether to audit the column, defaults to <see langword="false"/>.</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public static PropertyBuilder IsAuditable(this PropertyBuilder propertyBuilder, bool enabled = false)
    {
        ArgumentNullException.ThrowIfNull(propertyBuilder);

        propertyBuilder.Metadata.SetAnnotation(AuditAnnotationNames.Include, enabled);
        return propertyBuilder;
    }

    /// <inheritdoc cref="IsAuditable(PropertyBuilder, bool)"/>
    /// <typeparam name="TProperty">The type of the property being configured.</typeparam>
    public static PropertyBuilder<TProperty> IsAuditable<TProperty>(this PropertyBuilder<TProperty> propertyBuilder, bool enabled = false)
        => (PropertyBuilder<TProperty>)((PropertyBuilder)propertyBuilder).IsAuditable(enabled);

    #endregion Public Methods

    #region Internal Methods

    /// <summary>
    /// Gets whether the current program is running at design time.
    /// </summary>
    internal static bool IsDesignTime => System.Reflection.Assembly.GetEntryAssembly() is null;

    /// <summary>
    /// Configure properties of auditable entities.
    /// </summary>
    /// <param name="builder">Entity model builder.</param>
    /// <param name="comments">Comments to the audited entities.</param>
    /// <returns>The identity type of user for current <see cref="DbContext"/>.</returns>
    internal static Type? ConfigureAuditableEntities(this ModelBuilder builder, AuditEntityComments comments)
    {
        Type? entityClrType = null;
        Type? userKeyType = null;
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            var type = entityType.ClrType;
            var auditMetadata = entityType.GetAuditMetadata();

            if (entityType.HasProperty(Constants.Id, out var propertyType))
            {
                builder.Entity(type)
                    .Property(propertyType, Constants.Id)
                    .HasColumnOrder(0)
                    .HasComment(comments.Id);
            }

            if (entityType.HasProperty(Constants.CreationTime, out propertyType))
            {
                builder.Entity(type)
                    .Property(propertyType, Constants.CreationTime)
                    .HasColumnOrder(991)
                    .HasComment(comments.CreationTime);

                CheckDateTimeOffset(type, Constants.CreationTime, propertyType);
                auditMetadata.HasCreationTime = true;
            }
            if (entityType.HasProperty(Constants.CreatorId, out var cUserKeyType))
            {
                builder.Entity(type)
                    .Property(cUserKeyType, Constants.CreatorId)
                    .HasColumnOrder(992)
                    .HasComment(comments.CreatorId);

                CheckAndAssign(ref entityClrType, ref userKeyType, type, cUserKeyType);
                auditMetadata.UserKeyType = cUserKeyType;
                auditMetadata.HasCreatorId = true;
            }

            if (entityType.HasProperty(Constants.LastModificationTime, out propertyType))
            {
                builder.Entity(type)
                    .Property(propertyType, Constants.LastModificationTime)
                    .HasColumnOrder(994)
                    .HasComment(comments.LastModificationTime);

                CheckDateTimeOffset(type, Constants.LastModificationTime, propertyType);
                auditMetadata.HasLastModificationTime = true;
            }
            if (entityType.HasProperty(Constants.LastModifierId, out var mUserKeyType))
            {
                builder.Entity(type)
                    .Property(mUserKeyType, Constants.LastModifierId)
                    .HasColumnOrder(995)
                    .HasComment(comments.LastModifierId);

                CheckAndAssign(ref entityClrType, ref userKeyType, type, mUserKeyType);
                auditMetadata.UserKeyType = mUserKeyType;
                auditMetadata.HasLastModifierId = true;
            }

            if (entityType.HasProperty(Constants.IsDeleted, out propertyType))
            {
                builder.Entity(type)
                    .Property(propertyType, Constants.IsDeleted)
                    .HasColumnOrder(997)
                    .HasComment(comments.IsDeleted);

                CheckBoolean(type, Constants.IsDeleted, propertyType);
                auditMetadata.HasIsDeleted = true;
            }
            if (entityType.HasProperty(Constants.DeletionTime, out propertyType))
            {
                builder.Entity(type)
                    .Property(propertyType, Constants.DeletionTime)
                    .HasColumnOrder(998)
                    .HasComment(comments.DeletionTime);

                CheckDateTimeOffset(type, Constants.DeletionTime, propertyType);
                auditMetadata.HasDeletionTime = true;
            }
            if (entityType.HasProperty(Constants.DeleterId, out var dUserKeyType))
            {
                builder.Entity(type)
                    .Property(dUserKeyType, Constants.DeleterId)
                    .HasColumnOrder(999)
                    .HasComment(comments.DeleterId);

                CheckAndAssign(ref entityClrType, ref userKeyType, type, dUserKeyType);
                auditMetadata.UserKeyType = dUserKeyType;
                auditMetadata.HasDeleterId = true;
            }

            entityType.SetAuditMetadata(auditMetadata);

            if (IsDesignTime)
            {
                entityType.RemoveAnnotation(AuditAnnotationNames.Metadata);
            }
        }

        return userKeyType;
    }

    //internal static void ConfigureAuditLogEntities(this ModelBuilder builder, Type? userKeyType)
    //{
    //    var type = userKeyType is null
    //        ? typeof(string)
    //        : userKeyType.IsValueType
    //        ? typeof(Nullable<>).MakeGenericType(userKeyType)
    //        : userKeyType;

    //    builder.SharedTypeEntity<Dictionary<string, object?>>(
    //        AuditConfiguration.EntityChangeAuditLogTableName,
    //        b =>
    //        {
    //            b.ToTable(AuditConfiguration.EntityChangeAuditLogTableName)
    //                .HasComment("A table that stores entity changes.");
    //            b.HasKey("Id");
    //            b.Property<long>("Id")
    //                .ValueGeneratedOnAdd()
    //                .HasComment("The primary key for this entity.");
    //            b.Property<string?>("Schema")
    //                .IsUnicode(false)
    //                .HasMaxLength(64)
    //                .HasComment("The database schema name.");
    //            b.Property<string?>("Table")
    //                .IsUnicode(false)
    //                .HasMaxLength(128)
    //                .HasComment("The table name.");
    //            b.Property<string>("PrimaryKey")
    //                .IsUnicode(false)
    //                .HasMaxLength(128)
    //                .HasComment("The primary key of changed entity.");
    //            b.Property<string>("EntityType")
    //                .IsUnicode(false)
    //                .HasMaxLength(64)
    //                .HasComment("The type of changed entity.");
    //            b.Property<EventType>("EventType")
    //                .IsUnicode(false)
    //                .HasMaxLength(16)
    //                .HasConversion<string>()
    //                .HasComment("The type of audit event.");
    //            b.Property<DateTimeOffset>("EventTime")
    //                .HasComment("The time the audit event occurred.");
    //            b.Property(type, "OperatorId")
    //                .IsRequired(false)
    //                .HasComment("The identity of the user who change entity.");
    //            b.Property<string?>("OperatorName")
    //                .IsUnicode(true)
    //                .HasMaxLength(512)
    //                .IsRequired(false)
    //                .HasComment("The name of the user who change entity.");
    //            b.HasMany(AuditConfiguration.EntityFieldChangeAuditLogTableName)
    //             .WithOne
    //        }
    //    );

    //    builder.SharedTypeEntity<Dictionary<string, object?>>(
    //        AuditConfiguration.EntityFieldChangeAuditLogTableName,
    //        b =>
    //        {

    //        }
    //    );
    //}

    internal static bool GetAuditInclude(this IReadOnlyAnnotatable annotatable)
    {
        ArgumentNullException.ThrowIfNull(annotatable);

        var value = annotatable.FindAnnotation(AuditAnnotationNames.Include)?.Value;
        return annotatable switch
        {
            IEntityType => value is not null && (bool)value, // Entity auditable defaults to false
            IProperty => value is null || (bool)value, // Entity property or field auditable defaults to true
            _ => throw new InvalidOperationException()
        };
    }

    internal static void SetAuditMetadata(this IMutableEntityType entityType, AuditMetadata metadata)
    {
        ArgumentNullException.ThrowIfNull(entityType);
        ArgumentNullException.ThrowIfNull(metadata);

        if (entityType.FindAnnotation(AuditAnnotationNames.Metadata)?.Value is AuditMetadata original)
        {
            original.Append(metadata);
            entityType.SetAnnotation(AuditAnnotationNames.Metadata, original);
        }
        else
        {
            entityType.SetAnnotation(AuditAnnotationNames.Metadata, metadata);
        }
    }

    internal static AuditMetadata GetAuditMetadata(this IReadOnlyAnnotatable entityType)
    {
        ArgumentNullException.ThrowIfNull(entityType);

        return entityType.FindAnnotation(AuditAnnotationNames.Metadata)?.Value is AuditMetadata metadata
            ? metadata
            : new AuditMetadata();
    }

    #endregion Internal Methods

    #region Private Methods

    private static void CheckAndAssign(ref Type? entityType, ref Type? userKeyType, Type currentEntityType, Type currentUserKeytype)
    {
        if (userKeyType is null)
        {
            entityType = currentEntityType;
            userKeyType = currentUserKeytype;
        }
        else if (userKeyType != currentUserKeytype)
        {
            const string msg = "All audit entities in the same 'DbContext' cannot have different user's identity types.";
            var desc = entityType == currentEntityType
                ? string.Format(
                    "Entity type '{0}' has both type '{1}' and type '{2}' as user's identity type.",
                    currentEntityType.GetFriendlyName(),
                    userKeyType.GetFriendlyName(),
                    currentUserKeytype.GetFriendlyName())
                : string.Format(
                    "Entity type '{0}' has type '{1}' as user key type but entity '{2}' has type '{3}' as user's identity type.",
                    entityType!.GetFriendlyName(),
                    userKeyType.GetFriendlyName(),
                    currentEntityType.GetFriendlyName(),
                    currentUserKeytype.GetFriendlyName());
            throw new InvalidOperationException($"{msg} {desc}");
        }
    }

    private static void CheckDateTimeOffset(Type currentEntityType, string propertyName, Type propertyType)
    {
        if (!propertyType.IsAssignableFrom(typeof(DateTimeOffset)))
        {
            throw new InvalidOperationException(string.Format(
                "Type '{2}' of member '{1}' in entity type '{0}' is neither 'DateTimeOffset' nor 'DateTimeOffset?'.",
                currentEntityType.GetFriendlyName(),
                propertyName,
                propertyType.GetFriendlyName()));
        }
    }

    private static void CheckBoolean(Type currentEntityType, string propertyName, Type propertyType)
    {
        if (!propertyType.IsAssignableFrom(typeof(bool)))
        {
            throw new InvalidOperationException(string.Format(
                "Type '{2}' of member '{1}' in entity type '{0}' is not 'bool'.",
                currentEntityType.GetFriendlyName(),
                propertyName,
                propertyType.GetFriendlyName()));
        }
    }

    private static bool HasProperty(this IMutableEntityType entityType, string propertyName)
    {
        return entityType.FindProperty(propertyName) is not null;
    }

    private static bool HasProperty(this IMutableEntityType entityType, string propertyName, [NotNullWhen(true)] out Type? type)
    {
        type = entityType.FindProperty(propertyName)?.ClrType;
        return type is not null;
    }

    #endregion Private Methods
}
