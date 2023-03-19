using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Ling.EntityFrameworkCore.Audit.Internal.Extensions;

internal sealed class AuditOptionsExtension : IDbContextOptionsExtension
{
    public Action<AuditOptions>? Action { get; }

    /// <inheritdoc/>
    public DbContextOptionsExtensionInfo Info => new AuditOptionsExtensionInfo(this);

    public AuditOptionsExtension(Action<AuditOptions>? setupAction)
    {
        Action = setupAction;
    }

    public AuditOptionsExtension([NotNull] AuditOptionsExtension copyFrom)
    {
        Action = copyFrom.Action;
    }

    /// <inheritdoc/>
    public void ApplyServices(IServiceCollection services)
    {
    }

    /// <inheritdoc/>
    public void Validate(IDbContextOptions options)
    {
    }

    private class AuditOptionsExtensionInfo : DbContextOptionsExtensionInfo
    {
        public override bool IsDatabaseProvider { get; }
        public override string LogFragment { get; } = string.Empty;

        public AuditOptionsExtensionInfo(IDbContextOptionsExtension extension) : base(extension)
        {
        }

        public override int GetServiceProviderHashCode() => 0;

        public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other) => other is AuditOptionsExtensionInfo;

        public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
        {
        }
    }
}
