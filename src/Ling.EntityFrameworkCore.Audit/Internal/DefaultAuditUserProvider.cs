using Ling.EntityFrameworkCore.Audit.Internal.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Ling.EntityFrameworkCore.Audit.Internal;

/// <summary>
/// Default implementation of interface <see cref="IAuditUserProvider{T}"/>.
/// </summary>
internal sealed class DefaultAuditUserProvider : IAuditUserProvider<string>
{
    private readonly ILogger<DefaultAuditUserProvider> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AuditOptions _options;

    /// <inheritdoc/>
    public string? Id
    {
        get
        {
            var id = _httpContextAccessor.HttpContext?.User.FindFirstValue(_options.UserIdClaimType);
            return string.IsNullOrWhiteSpace(id) ? null : id;
        }
    }

    /// <inheritdoc/>
    public string? Name => _httpContextAccessor.HttpContext?.User.FindFirstValue(_options.UserNameClaimType);

    public DefaultAuditUserProvider(ICurrentDbContext current, IDbContextOptions options, ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<DefaultAuditUserProvider>();
        _httpContextAccessor = current.Context.GetService<IHttpContextAccessor>();

        var configuration = current.Context.GetService<IConfiguration>();
        _options = configuration.GetSection("Audit").Get<AuditOptions>() ?? new AuditOptions();

        var action = options?.FindExtension<AuditOptionsExtension>()?.Action;

        if (action is not null)
        {
            _logger.LogDebug("The 'Action' in 'AuditOptionsExtension' is found, apply it to the '_options'.");

            action.Invoke(_options);
        }
        else
        {
            _logger.LogDebug("The 'Action' in 'AuditOptionsExtension' is null, use the 'AuditOptions' in the configuration file.");
        }
    }
}
