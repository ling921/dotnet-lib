using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Ling.EntityFrameworkCore.Audit.Internal.Extensions;

internal class AuditConventionExtension : IDbContextOptionsExtension
{
    /// <inheritdoc/>
    public DbContextOptionsExtensionInfo Info => new ExtensionInfo(this);

    /// <inheritdoc/>
    public void ApplyServices(IServiceCollection services)
    {
        services.TryAddEnumerable(ServiceDescriptor.Scoped<IConventionSetPlugin, AuditConventionSetPlugin>());
    }

    /// <inheritdoc/>
    public void Validate(IDbContextOptions options)
    {
    }
}
