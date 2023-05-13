using System.Security.Claims;

namespace Ling.Blazor.Authentication.JwtBearer;

/// <summary>
/// A class that represents JWT authentication options.
/// </summary>
public sealed class JwtAuthOptions
{
    /// <summary>
    /// Gets or sets the name of the authentication scheme, which defaults to "Bearer".
    /// </summary>
    public string AuthenticationScheme { get; set; } = "Bearer";

    /// <summary>
    /// Gets or sets the type of the claim that represents the user identifier, which defaults to ClaimTypes.NameIdentifier.
    /// </summary>
    public string UserIdClaimType { get; set; } = ClaimTypes.NameIdentifier;

    /// <summary>
    /// Gets or sets the type of the claim that represents the user role, which defaults to ClaimTypes.Role.
    /// </summary>
    public string RoleClaimType { get; set; } = ClaimTypes.Role;

    /// <summary>
    /// Gets or sets the key used to store the user token in a persistence store.
    /// </summary>
    public string TokenPersistenceKey { get; set; } = "Ling.UserToken";
}
