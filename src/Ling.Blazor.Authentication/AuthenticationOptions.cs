using System.Security.Claims;

namespace Ling.Blazor.Authentication;

/// <summary>
/// A class that represents authentication options.
/// </summary>
public sealed class AuthenticationOptions
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
}
