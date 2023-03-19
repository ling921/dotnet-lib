using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Ling.EntityFrameworkCore.Audit.Internal.Extensions;

internal class AuditConventionExtension : IDbContextOptionsExtension
{
    /// <inheritdoc/>
    public DbContextOptionsExtensionInfo Info => new AuditConventionExtensionInfo(this);

    /// <inheritdoc/>
    public void ApplyServices(IServiceCollection services)
    {
        services.TryAddEnumerable(ServiceDescriptor.Scoped<IConventionSetPlugin, AuditConventionSetPlugin>());
    }

    /// <inheritdoc/>
    public void Validate(IDbContextOptions options)
    {
    }

    private class AuditConventionExtensionInfo : DbContextOptionsExtensionInfo
    {
        public override bool IsDatabaseProvider { get; }
        public override string LogFragment { get; } = string.Empty;

        public AuditConventionExtensionInfo(IDbContextOptionsExtension extension) : base(extension)
        {
        }

        public override int GetServiceProviderHashCode() => 0;

        public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other) => other is AuditConventionExtensionInfo;

        public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
        {
        }
    }
}
