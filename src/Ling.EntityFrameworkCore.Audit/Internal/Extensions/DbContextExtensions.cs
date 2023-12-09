using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace Ling.EntityFrameworkCore.Audit.Internal.Extensions;

internal static class DbContextExtensions
{
    internal static AuditOptions GetAuditOptions(this DbContext context)
    {
        var configuration = context.GetService<IConfiguration>();
        var options = configuration.GetSection(AuditDefaults.ConfigurationKey).Get<AuditOptions>() ?? new();

        var extension = context.GetService<IDbContextOptions>()
            .Extensions
            .OfType<AuditOptionsExtension>()
            .FirstOrDefault();

        if (extension?.Action is not null)
        {
            extension.Action.Invoke(options);
        }

        return options;
    }
}
