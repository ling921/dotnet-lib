using System.Security.Claims;

namespace Ling.Blazor.Authentication;

/// <summary>
/// Authentication options.
/// </summary>
public sealed class AuthOptions
{
    /// <summary>
    /// Gets or sets the claim type for user ID.
    /// </summary>
    public string UserIdClaimType { get; set; } = ClaimTypes.NameIdentifier;

    /// <summary>
    /// Gets or sets the claim type for user role.
    /// </summary>
    public string RoleClaimType { get; set; } = ClaimTypes.Role;
}
