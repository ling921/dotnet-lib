namespace Ling.EntityFrameworkCore.Audit.Models;

/// <summary>
/// Represents the detail of <see cref="AuditLog"/>.
/// </summary>
public class AuditLogDetail
{
    /// <summary>
    /// Gets or sets the primary key.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the primary key of <see cref="AuditLog"/>.
    /// </summary>
    public long AuditLogId { get; set; }

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
