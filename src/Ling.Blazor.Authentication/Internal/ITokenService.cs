using Microsoft.AspNetCore.Components.Authorization;

namespace Ling.Blazor.Authentication.Internal;

/// <summary>
/// Token information service interface.
/// </summary>
internal interface ITokenService
{
    /// <summary>
    /// Save the token information to browser storage.
    /// <para>This method will call <see cref="AuthenticationStateProvider.NotifyAuthenticationStateChanged(Task{AuthenticationState})"/> automaticly.</para>
    /// </summary>
    /// <param name="token">The token to save.</param>
    /// <param name="cancellationToken">Optional. The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
    Task SetTokenAsync(TokenInfo token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the token information from browser storage.
    /// </summary>
    /// <param name="cancellationToken">Optional. The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation. The task result contains the access token.</returns>
    Task<TokenInfo?> GetTokenAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Clear the token information in browser storage.
    /// <para>This method will call <see cref="AuthenticationStateProvider.NotifyAuthenticationStateChanged(Task{AuthenticationState})"/> automaticly.</para>
    /// </summary>
    /// <param name="cancellationToken">Optional. The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
    Task ClearTokenAsync(CancellationToken cancellationToken = default);
}
