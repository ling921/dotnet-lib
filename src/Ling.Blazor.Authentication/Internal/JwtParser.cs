using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Text.Json;

namespace Ling.Blazor.Authentication.Internal;

/// <summary>
/// Extension method for JWT.
/// </summary>
internal static class JwtParser
{
    /// <summary>
    /// Tries to read user claims principal from JWT string.
    /// </summary>
    /// <param name="token">The JWT string.</param>
    /// <param name="options">The authentication options.</param>
    /// <param name="userPrincipal">The user claims principal in JWT.</param>
    /// <returns><see langword="true"/> if read successfully; otherwise, <see langword="false"/>.</returns>
    public static bool TryRead(string? token, AuthenticationOptions options, [NotNullWhen(true)] out ClaimsPrincipal? userPrincipal)
    {
        if (!string.IsNullOrWhiteSpace(token))
        {
            try
            {
                var claims = new List<Claim>();
                var payload = token.Split('.')[1];
                var jsonBytes = Base64UrlDecode(payload);

                var jsonDocument = JsonDocument.Parse(jsonBytes);
                foreach (var jsonProperty in jsonDocument.RootElement.EnumerateObject())
                {
                    ReadJsonProperty(claims, jsonProperty);
                }

                //Console.Out.WriteLine<List<Claim>>(claims);
                if (claims.Find(c => c.Type == "exp") is Claim ec &&
                    long.TryParse(ec.Value, out var expires) &&
                    new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(expires) <= DateTime.UtcNow)
                {
                    userPrincipal = null;
                    return false;
                }

                userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Bearer", options.UserIdClaimType, options.RoleClaimType));
                return true;
            }
            catch { }
        }

        userPrincipal = null;
        return false;
    }

    private static byte[] Base64UrlDecode(string base64UrlEncodedString)
        => (base64UrlEncodedString.Length % 4) switch
        {
            2 => Convert.FromBase64String(base64UrlEncodedString + "=="),
            3 => Convert.FromBase64String(base64UrlEncodedString + "="),
            _ => Convert.FromBase64String(base64UrlEncodedString),
        };

    private static void ReadJsonProperty(List<Claim> claims, JsonProperty jsonProperty)
    {
        switch (jsonProperty.Value.ValueKind)
        {
            case JsonValueKind.Undefined:
                break;
            case JsonValueKind.Object:
                break;
            case JsonValueKind.Array:
                foreach (var jsonArrayItem in jsonProperty.Value.EnumerateArray())
                {
                    switch (jsonArrayItem.ValueKind)
                    {
                        case JsonValueKind.Undefined:
                            break;
                        case JsonValueKind.Object:
                            break;
                        case JsonValueKind.Array:
                            break;
                        case JsonValueKind.String:
                            {
                                var jsonArrayItemValue = jsonArrayItem.GetString();
                                claims.Add(new Claim(jsonProperty.Name, jsonArrayItemValue ?? string.Empty));
                            }
                            break;
                        case JsonValueKind.Number:
                            {
                                var jsonArrayItemValue = jsonArrayItem.GetDouble();
                                claims.Add(new Claim(jsonProperty.Name, jsonArrayItemValue.ToString(), ClaimValueTypes.Double));
                            }
                            break;
                        case JsonValueKind.True:
                        case JsonValueKind.False:
                            {
                                var jsonArrayItemValue = jsonArrayItem.GetBoolean();
                                claims.Add(new Claim(jsonProperty.Name, jsonArrayItemValue.ToString(), ClaimValueTypes.Boolean));
                            }
                            break;
                        case JsonValueKind.Null:
                            break;
                        default:
                            break;
                    }
                }
                break;
            case JsonValueKind.String:
                {
                    var jsonPropertyValue = jsonProperty.Value.GetString();
                    claims.Add(new Claim(jsonProperty.Name, jsonPropertyValue ?? string.Empty));
                }
                break;
            case JsonValueKind.Number:
                {
                    var jsonPropertyValue = jsonProperty.Value.GetDouble();
                    claims.Add(new Claim(jsonProperty.Name, jsonPropertyValue.ToString(), ClaimValueTypes.Double));
                }
                break;
            case JsonValueKind.True:
            case JsonValueKind.False:
                {
                    var jsonPropertyValue = jsonProperty.Value.GetBoolean();
                    claims.Add(new Claim(jsonProperty.Name, jsonPropertyValue.ToString(), ClaimValueTypes.Boolean));
                }
                break;
            case JsonValueKind.Null:
                break;
            default:
                break;
        }
    }
}
