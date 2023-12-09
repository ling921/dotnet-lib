using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Ling.EntityFrameworkCore.Audit.Internal.Extensions;

internal sealed class UserProviderExtension<TUserProvider, TUserKey> : IDbContextOptionsExtension
    where TUserProvider : class, IAuditUserProvider<TUserKey>
{
    /// <inheritdoc/>
    public DbContextOptionsExtensionInfo Info => new UserProviderExtensionInfo(this);

    /// <inheritdoc/>
    public void ApplyServices(IServiceCollection services)
    {
        services.TryAddScoped<IAuditUserProvider<TUserKey>, TUserProvider>();
    }

    /// <inheritdoc/>
    public void Validate(IDbContextOptions options)
    {
    }

    private class UserProviderExtensionInfo : DbContextOptionsExtensionInfo
    {
        public override bool IsDatabaseProvider { get; }
        public override string LogFragment { get; } = string.Empty;

        public UserProviderExtensionInfo(IDbContextOptionsExtension extension) : base(extension)
        {
        }

        public override int GetServiceProviderHashCode() => 0;

        public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other) => other is UserProviderExtensionInfo;

        public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
        {
        }
    }
}
