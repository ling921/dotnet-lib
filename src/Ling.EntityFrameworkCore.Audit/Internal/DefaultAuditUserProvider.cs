using Ling.EntityFrameworkCore.Audit.Internal.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Infrastructure;
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
            _logger.LogDebug("Get current user id: {Id}", id);
            return string.IsNullOrWhiteSpace(id) ? null : id;
        }
    }

    /// <inheritdoc/>
    public string? Name => _httpContextAccessor.HttpContext?.User.FindFirstValue(_options.UserNameClaimType);

    public DefaultAuditUserProvider(ICurrentDbContext current, ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<DefaultAuditUserProvider>();
        _httpContextAccessor = current.Context.GetService<IHttpContextAccessor>();
        _options = current.Context.GetAuditOptions();
    }
}
