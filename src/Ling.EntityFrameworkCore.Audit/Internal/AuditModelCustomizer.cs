using Ling.EntityFrameworkCore.Audit.Internal.Extensions;
using Ling.EntityFrameworkCore.Audit.TypeConfigurations;
using Ling.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Ling.EntityFrameworkCore.Audit.Internal;

internal sealed class AuditModelCustomizer<TUserKey> : ModelCustomizer
{
    private readonly IDbContextOptions _options;
    private readonly ILogger _logger;

    public AuditModelCustomizer(
        ModelCustomizerDependencies dependencies,
        IDbContextOptions options,
        ILoggerFactory loggerFactory) : base(dependencies)
    {
        _options = options;
        _logger = loggerFactory.CreateLogger<AuditModelCustomizer<TUserKey>>();
    }

    /// <inheritdoc/>
    public override void Customize(ModelBuilder modelBuilder, DbContext context)
    {
        base.Customize(modelBuilder, context);

        AuditOptions auditOptions;
        var action = _options?.FindExtension<AuditOptionsExtension>()?.Action;
        if (action is null)
        {
            var configuration = context.GetService<IConfiguration>();
            auditOptions = configuration.GetSection("Audit").Get<AuditOptions>() ?? new AuditOptions();
        }
        else
        {
            auditOptions = new AuditOptions();
            action.Invoke(auditOptions);
        }

        modelBuilder.ApplyConfiguration(new AuditLogTypeConfiguration<TUserKey>());
        modelBuilder.ApplyConfiguration(new AuditLogDetailTypeConfiguration());

        modelBuilder.ConfigureAuditEntities(auditOptions.Comments);

        _logger.LogInformation("Complete the audit entity configuration.");
    }
}
