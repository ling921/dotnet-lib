namespace Ling.EntityFrameworkCore.Audit.Models;

/// <summary>
/// Represents the detail of <see cref="AuditEntityLog{TUserKey}"/>.
/// </summary>
public class AuditFieldLog
{
    /// <summary>
    /// Gets or sets the primary key.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the primary key of <see cref="AuditEntityLog{TUserKey}"/>.
    /// </summary>
    public long EntityLogId { get; set; }

    /// <summary>
    /// Gets or sets the property name.
    /// </summary>
    public string PropertyName { get; set; } = null!;

    /// <summary>
    /// Gets or sets the original value.
    /// </summary>
    public string? OriginalValue { get; set; }

    /// <summary>
    /// Gets or sets the new value.
    /// </summary>
    public string? NewValue { get; set; }
}
