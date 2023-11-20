using Ling.EntityFrameworkCore.Audit;
using Ling.EntityFrameworkCore.Audit.Internal;
using Ling.EntityFrameworkCore.Audit.Internal.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Ling.EntityFrameworkCore.Extensions;

/// <summary>
/// Extension methods for <see cref="DbContextOptionsBuilder"/>.
/// </summary>
public static class DbContextOptionsBuilderExtensions
{
    /// <summary>
    /// Sets the <see cref="ISaveChangesInterceptor"/>, <see cref="IModelCustomizer"/>, <see
    /// cref="AuditOptions"/> to be used for the auditing.
    /// </summary>
    /// <param name="builder">The <see cref="DbContextOptionsBuilder"/>.</param>
    /// <param name="setupAction">The action used to configure the <see cref="AuditOptions"/>.</param>
    public static DbContextOptionsBuilder UseAudit(this DbContextOptionsBuilder builder, Action<AuditOptions>? setupAction = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.UseAuditOption(setupAction);
        builder.UseAuditUserProvider<DefaultAuditUserProvider, string>();
        builder.UseAuditConvention();

        builder.AddInterceptors(new AuditInterceptor<string>());
        builder.ReplaceService<IModelCustomizer, AuditModelCustomizer<string>>();

        return builder;
    }

    /// <summary>
    /// Sets the <see cref="ISaveChangesInterceptor"/>, <see cref="IModelCustomizer"/>, <see
    /// cref="AuditOptions"/> to be used for the auditing.
    /// </summary>
    /// <param name="builder">The <see cref="DbContextOptionsBuilder"/>.</param>
    /// <param name="setupAction">The action used to configure the <see cref="AuditOptions"/>.</param>
    public static DbContextOptionsBuilder UseAudit<TUserProvider, TUserKey>(
        this DbContextOptionsBuilder builder,
        Action<AuditOptions>? setupAction = null)
        where TUserProvider : class, IAuditUserProvider<TUserKey>
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.UseAuditOption(setupAction);
        builder.UseAuditUserProvider<TUserProvider, TUserKey>();
        builder.UseAuditConvention();

        builder.AddInterceptors(new AuditInterceptor<TUserKey>());
        builder.ReplaceService<IModelCustomizer, AuditModelCustomizer<TUserKey>>();

        return builder;
    }

    private static DbContextOptionsBuilder UseAuditOption(this DbContextOptionsBuilder builder, Action<AuditOptions>? setupAction)
    {
        var extension = builder.Options.FindExtension<AuditOptionsExtension>() ?? new AuditOptionsExtension(setupAction);

        ((IDbContextOptionsBuilderInfrastructure)builder).AddOrUpdateExtension(extension);

        return builder;
    }

    private static DbContextOptionsBuilder UseAuditUserProvider<TUserProvider, TUserKey>(this DbContextOptionsBuilder builder)
            where TUserProvider : class, IAuditUserProvider<TUserKey>
    {
        var extension = builder.Options.FindExtension<UserProviderExtension<TUserProvider, TUserKey>>()
            ?? new UserProviderExtension<TUserProvider, TUserKey>();

        ((IDbContextOptionsBuilderInfrastructure)builder).AddOrUpdateExtension(extension);

        return builder;
    }

    private static DbContextOptionsBuilder UseAuditConvention(this DbContextOptionsBuilder builder)
    {
        var extension = builder.Options.FindExtension<AuditConventionExtension>() ?? new AuditConventionExtension();

        ((IDbContextOptionsBuilderInfrastructure)builder).AddOrUpdateExtension(extension);

        return builder;
    }
}
