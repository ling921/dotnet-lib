namespace Ling.Blazor.Authentication.JwtBearer;

/// <summary>
/// Defines the interface for a service that manages JWT tokens.
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Sets the specified JWT token in the user's browser storage.
    /// </summary>
    /// <param name="token">The token to set.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task SetTokenAsync(JwtTokenInfo token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the JWT token from the user's browser storage, if present.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to cancel the operation.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation that returns the current token, or <c>null</c> if there is no token available.</returns>
    Task<JwtTokenInfo?> GetTokenAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes the JWT token from the user's browser storage.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task ClearTokenAsync(CancellationToken cancellationToken = default);
}
