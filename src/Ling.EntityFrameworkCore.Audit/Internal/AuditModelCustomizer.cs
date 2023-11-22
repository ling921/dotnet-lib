using Ling.EntityFrameworkCore.Audit.Internal.Extensions;
using Ling.EntityFrameworkCore.Audit.TypeConfigurations;
using Ling.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Ling.EntityFrameworkCore.Audit.Internal;

internal sealed class AuditModelCustomizer<TUserKey> : ModelCustomizer
{
    private readonly ILogger _logger;

    public AuditModelCustomizer(
        ModelCustomizerDependencies dependencies,
        ILoggerFactory loggerFactory) : base(dependencies)
    {
        _logger = loggerFactory.CreateLogger<AuditModelCustomizer<TUserKey>>();
    }

    /// <inheritdoc/>
    public override void Customize(ModelBuilder modelBuilder, DbContext context)
    {
        base.Customize(modelBuilder, context);

        var auditOptions = context.GetAuditOptions();

        modelBuilder.ApplyConfiguration(new AuditEntityLogTypeConfiguration<TUserKey>());
        modelBuilder.ApplyConfiguration(new AuditFieldLogTypeConfiguration());

        modelBuilder.ConfigureAuditableEntities(auditOptions.Comments);

        _logger.LogInformation("Complete the audit entity model configuration.");
    }
}
