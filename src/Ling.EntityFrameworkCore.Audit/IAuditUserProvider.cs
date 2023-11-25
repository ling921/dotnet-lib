namespace Ling.EntityFrameworkCore.Audit;

/// <summary>
/// Interface for audit user information.
/// </summary>
public interface IAuditUserProvider<TUserKey>
{
    /// <summary>
    /// Gets the identity of user.
    /// </summary>
    public TUserKey? Id { get; }

    /// <summary>
    /// Gets the name of user.
    /// </summary>
    public string? Name { get; }
}
