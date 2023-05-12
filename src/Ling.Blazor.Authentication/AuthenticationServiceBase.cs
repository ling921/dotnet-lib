using Ling.Blazor.Authentication.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Ling.Blazor.Authentication;

/// <summary>
/// An abstract base class for implementing authentication services.
/// </summary>
public abstract class AuthenticationServiceBase : IAuthenticationService
{
    /// <summary>
    /// Gets the service provider for dependency injection.
    /// </summary>
    protected readonly IServiceProvider ServiceProvider;

    private readonly ITokenService _tokenService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationServiceBase"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider for dependency injection.</param>
    protected AuthenticationServiceBase(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        _tokenService = ServiceProvider.GetRequiredService<ITokenService>();
    }

    /// <inheritdoc/>
    /// <remarks>Should call method 'SetTokenAsync'</remarks>
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
    protected async Task SetTokenAsync(TokenInfo token, CancellationToken cancellationToken = default)
    {
        await _tokenService.SetTokenAsync(token, cancellationToken);
    }
}
