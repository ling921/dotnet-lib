using Microsoft.Extensions.DependencyInjection;

namespace Ling.Blazor.Authentication.JwtBearer;

/// <summary>
/// An abstract base class for implementing JWT authentication services.
/// </summary>
public abstract class JwtAuthenticationServiceBase : IJwtAuthenticationService
{
    /// <summary>
    /// Gets the service provider for dependency injection.
    /// </summary>
    protected readonly IServiceProvider ServiceProvider;

    private readonly IJwtTokenService _tokenService;

    /// <summary>
    /// Initializes a new instance of the <see cref="JwtAuthenticationServiceBase"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider for dependency injection.</param>
    protected JwtAuthenticationServiceBase(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        _tokenService = ServiceProvider.GetRequiredService<IJwtTokenService>();
    }

    /// <inheritdoc/>
    /// <remarks>Implementations should call the method 'SetTokenAsync' to set the authentication token for the user.</remarks>
    public abstract Task LoginAsync(string username, string password, bool isPersistent = false, CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public virtual Task LogoutAsync(CancellationToken cancellationToken = default)
    {
        return _tokenService.ClearTokenAsync(cancellationToken);
    }

    /// <summary>
    /// Sets the token information for the current user.
    /// </summary>
    /// <param name="token">The token information to set.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    protected virtual async Task SetTokenAsync(JwtTokenInfo token, CancellationToken cancellationToken = default)
    {
        await _tokenService.SetTokenAsync(token, cancellationToken);
    }
}
