using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;

namespace Ling.Blazor.Authentication.Internal;

/// <summary>
/// Provides information about the authentication state of the current user.
/// </summary>
internal class AppAuthenticationStateProvider : AuthenticationStateProvider, ITokenService
{
    private const string _tokenKey = "Ling.UserToken";
    private readonly ILocalStorageService _localStorage;
    private readonly IOptions<AuthenticationOptions> _optionsAccessor;

    public AppAuthenticationStateProvider(
        ILocalStorageService localStorage,
        IOptions<AuthenticationOptions> optionsAccessor)
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
    public async Task SetTokenAsync(TokenInfo token, CancellationToken cancellationToken = default)
    {
        await _localStorage.SetItemAsync(_tokenKey, token, cancellationToken);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    /// <inheritdoc/>
    public async Task<TokenInfo?> GetTokenAsync(CancellationToken cancellationToken = default)
    {
        return await _localStorage.GetItemAsync<TokenInfo>(_tokenKey, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task ClearTokenAsync(CancellationToken cancellationToken = default)
    {
        await _localStorage.RemoveItemAsync(_tokenKey, cancellationToken);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
