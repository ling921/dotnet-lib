using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Ling.EntityFrameworkCore.Audit.Models;

/// <summary>
/// Represents the audit log.
/// </summary>
public class AuditLog
{
    /// <summary>
    /// Gets or sets the primary key.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the schema of database.
    /// </summary>
    public string? Schema { get; set; }

    /// <summary>
    /// Gets or sets the table of data changed.
    /// </summary>
    public string? Table { get; set; }

    /// <summary>
    /// Gets or sets the primary key of the <see cref="EntityEntry"/>.
    /// </summary>
    public string PrimaryKey { get; set; } = null!;

    /// <summary>
    /// Gets or sets the type of changed entity.
    /// </summary>
    public string EntityType { get; set; } = null!;

    /// <summary>
    /// Gets or sets the type of audit event.
    /// </summary>
    public EventType EventType { get; set; }

    /// <summary>
    /// Gets or sets the time the audit event occurred.
    /// </summary>
    public DateTimeOffset EventTime { get; set; }

    /// <summary>
    /// Gets or sets the identity of user who change entity.
    /// </summary>
    public string? OperatorId { get; set; }

    /// <summary>
    /// Gets or sets the name of user who change entity.
    /// </summary>
    public string? OperatorName { get; set; }

    /// <summary>
    /// Gets or sets the details of this audit log.
    /// </summary>
    public virtual ICollection<AuditLogDetail> Details { get; set; } = new List<AuditLogDetail>();
}
