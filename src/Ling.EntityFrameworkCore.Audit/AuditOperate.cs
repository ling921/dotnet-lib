namespace Ling.EntityFrameworkCore.Audit;

/// <summary>
/// Enum that specifies audit operation.
/// </summary>
[Flags]
public enum AuditOperate : byte
{
    /// <summary>
    /// None.
    /// </summary>
    None = 0,

    /// <summary>
    /// Create.
    /// </summary>
    Create = 1 << 1,

    /// <summary>
    /// Update.
    /// </summary>
    Update = 1 << 2,

    /// <summary>
    /// Delete.
    /// </summary>
    Delete = 1 << 3,

    /// <summary>
    /// Create, Update and Delete.
    /// </summary>
    All = Create | Update | Delete,
}
