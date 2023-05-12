using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace Ling.Blazor.Authentication.Internal;

/// <inheritdoc/>
internal class AppAuthorizationMessageHandler : DelegatingHandler, IDisposable
{
    private readonly ITokenService _tokenService;
    private readonly IOptionsSnapshot<AuthenticationOptions> _optionsAccessor;

    private readonly AuthenticationStateChangedHandler? _authenticationStateChangedHandler;
    private TokenInfo? _lastToken;
    private AuthenticationHeaderValue? _cachedHeader;

    public AppAuthorizationMessageHandler(ITokenService tokenService, IOptionsSnapshot<AuthenticationOptions> optionsAccessor)
    {
        _tokenService = tokenService;
        _optionsAccessor = optionsAccessor;

        // Invalidate the cached _lastToken when the authentication state changes
        if (_tokenService is AuthenticationStateProvider authStateProvider)
        {
            _authenticationStateChangedHandler = _ => { _lastToken = null; };
            authStateProvider.AuthenticationStateChanged += _authenticationStateChangedHandler;
        }
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (_lastToken == null || DateTimeOffset.Now >= _lastToken.Expires.AddMinutes(-5))
        {
            var token = await _tokenService.GetTokenAsync(cancellationToken);

            if (token is not null)
            {
                _lastToken = token;
                _cachedHeader = new AuthenticationHeaderValue(_optionsAccessor.Value.AuthenticationScheme, _lastToken.AccessToken);
            }
            else
            {
                //throw new AccessTokenNotAvailableException(_navigation, tokenResult, _tokenOptions?.Scopes);
            }
        }

        // We don't try to handle 401s and retry the request with a new token automatically since that would mean we need to copy the request
        // headers and buffer the body and we expect that the user instead handles the 401s. (Also, we can't really handle all 401s as we might
        // not be able to provision a token without user interaction).
        request.Headers.Authorization = _cachedHeader;

        return await base.SendAsync(request, cancellationToken);
    }

    void IDisposable.Dispose()
    {
        if (_tokenService is AuthenticationStateProvider authStateProvider)
        {
            authStateProvider.AuthenticationStateChanged -= _authenticationStateChangedHandler;
        }
        Dispose(disposing: true);
    }
}
