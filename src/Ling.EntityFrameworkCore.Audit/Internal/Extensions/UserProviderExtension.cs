using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Ling.EntityFrameworkCore.Audit.Internal.Extensions;

internal sealed class UserProviderExtension<TUserProvider> : IDbContextOptionsExtension
    where TUserProvider : class, IAuditUserProvider
{
    /// <inheritdoc/>
    public DbContextOptionsExtensionInfo Info => new ExtensionInfo(this);

    /// <inheritdoc/>
    public void ApplyServices(IServiceCollection services)
    {
        services.TryAddScoped<IAuditUserProvider, TUserProvider>();
    }

    /// <inheritdoc/>
    public void Validate(IDbContextOptions options)
    {
    }
}
