namespace Ling.EntityFrameworkCore.Audit.Models;

/// <summary>
/// Enum that specifies audit event type.
/// </summary>
public enum EventType
{
    /// <summary>
    /// Default.
    /// </summary>
    None,

    /// <summary>
    /// Create entity.
    /// </summary>
    Create,

    /// <summary>
    /// Modify entity.
    /// </summary>
    Modify,

    /// <summary>
    /// Delete entity.
    /// </summary>
    Delete,

    /// <summary>
    /// Soft delete entity.
    /// </summary>
    SoftDelete,

    /// <summary>
    /// Recovery soft-deleted entity.
    /// </summary>
    Recovery,
}
