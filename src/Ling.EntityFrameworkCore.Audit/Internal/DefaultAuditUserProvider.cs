using Ling.EntityFrameworkCore.Audit.Extensions;
using Ling.EntityFrameworkCore.Audit.Internal.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Security.Claims;

namespace Ling.EntityFrameworkCore.Audit.Internal;

/// <summary>
/// Default implementation of interface <see cref="IAuditUserProvider"/>.
/// </summary>
internal sealed class DefaultAuditUserProvider : IAuditUserProvider
{
    private readonly ILogger<DefaultAuditUserProvider> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AuditOptions _options;

    /// <inheritdoc/>
    public string? Identity
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

        var action = options?.FindExtension<AuditOptionsExtension>()?.Action;

        if (action is null)
        {
            var configuration = current.Context.GetService<IConfiguration>();
            _options = configuration.GetSection("Audit").Get<AuditOptions>() ?? new AuditOptions();
        }
        else
        {
            _options = new AuditOptions();
            action.Invoke(_options);
        }
    }

    /// <summary>
    /// Get the user identity match specific type.
    /// </summary>
    /// <param name="type">The type to get.</param>
    /// <returns>The user identity.</returns>
    public object? GeIdentityOfType(Type type)
    {
        if (string.IsNullOrWhiteSpace(Identity) || type is null)
        {
            return null;
        }

        try
        {
            var underlyingType = Nullable.GetUnderlyingType(type) ?? type;
            if (type.IsPrimitive)
            {
                return Convert.ChangeType(Identity, underlyingType);
            }

            var method = underlyingType.GetMethod("Parse", new Type[] { typeof(string) });
            if (method is not null)
            {
                return method.Invoke(null, new object[] { Identity });
            }

            var converter = TypeDescriptor.GetConverter(underlyingType);
            if (converter is not null)
            {
                return converter.ConvertFrom(Identity);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while converting user identity '{identity}' to type '{type}'.", Identity, type.GetFriendlyName());
        }

        return null;
    }
}
