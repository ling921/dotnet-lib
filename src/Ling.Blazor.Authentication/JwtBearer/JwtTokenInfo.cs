using System.Text.Json.Serialization;

namespace Ling.Blazor.Authentication.JwtBearer;

/// <summary>
/// A record that represents the JWT token information of user.
/// </summary>
/// <param name="AccessToken">The access token.</param>
/// <param name="Expires">The expiration time of the token.</param>
/// <param name="RefreshToken">The refresh token.</param>
public record JwtTokenInfo(
    [property: JsonPropertyName("access_token")] string AccessToken,
    [property: JsonPropertyName("expires")] DateTimeOffset Expires,
    [property: JsonPropertyName("refresh_token")] string RefreshToken);
