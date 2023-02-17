using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Ling.EntityFrameworkCore.Audit.Internal.Extensions;

internal sealed class AuditOptionsExtension : IDbContextOptionsExtension
{
    public Action<AuditOptions>? Action { get; }

    /// <inheritdoc/>
    public DbContextOptionsExtensionInfo Info => new ExtensionInfo(this);

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
}
