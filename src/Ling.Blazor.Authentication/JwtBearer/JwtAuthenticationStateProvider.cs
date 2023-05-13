using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;

namespace Ling.Blazor.Authentication.JwtBearer;

/// <summary>
/// Implements authentication state management using JWT token stored in the browser's local storage.
/// </summary>
public class JwtAuthenticationStateProvider : AuthenticationStateProvider, IJwtTokenService
{
    private readonly ILocalStorageService _localStorage;
    private readonly IOptions<JwtAuthOptions> _optionsAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="JwtAuthenticationStateProvider"/> class.
    /// </summary>
    /// <param name="localStorage">The local storage service used to store and retrieve tokens.</param>
    /// <param name="optionsAccessor">The options accessor used to retrieve the authentication options.</param>
    public JwtAuthenticationStateProvider(
        ILocalStorageService localStorage,
        IOptions<JwtAuthOptions> optionsAccessor)
    {
        _localStorage = localStorage;
        _optionsAccessor = optionsAccessor;
    }

    /// <inheritdoc/>
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await GetTokenAsync();
        return JwtParser.TryRead(token?.AccessToken, _optionsAccessor.Value, out var claimsPrincipal)
            ? new AuthenticationState(claimsPrincipal)
            : new AuthenticationState(new());
    }

    /// <inheritdoc/>
    public virtual async Task SetTokenAsync(JwtTokenInfo token, CancellationToken cancellationToken = default)
    {
        await _localStorage.SetItemAsync(_optionsAccessor.Value.TokenPersistenceKey, token, cancellationToken);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    /// <inheritdoc/>
    public virtual async Task<JwtTokenInfo?> GetTokenAsync(CancellationToken cancellationToken = default)
    {
        return await _localStorage.GetItemAsync<JwtTokenInfo>(_optionsAccessor.Value.TokenPersistenceKey, cancellationToken);
    }

    /// <inheritdoc/>
    public virtual async Task ClearTokenAsync(CancellationToken cancellationToken = default)
    {
        await _localStorage.RemoveItemAsync(_optionsAccessor.Value.TokenPersistenceKey, cancellationToken);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
