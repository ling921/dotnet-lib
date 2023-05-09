using System.Text.Json.Serialization;

namespace Ling.Blazor.Authentication;

/// <summary>
/// A record that represents the information of a token
/// </summary>
/// <param name="AccessToken">The access token</param>
/// <param name="Expires">The expiration time of the token</param>
/// <param name="RefreshToken">The refresh token</param>
public record TokenInfo(
    [property: JsonPropertyName("access_token")] string AccessToken,
    [property: JsonPropertyName("expires")] DateTimeOffset Expires,
    [property: JsonPropertyName("refresh_token")] string RefreshToken);
