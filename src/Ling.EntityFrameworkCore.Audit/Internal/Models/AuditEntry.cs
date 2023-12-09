using Ling.EntityFrameworkCore.Audit.Internal.Extensions;
using Ling.EntityFrameworkCore.Audit.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Ling.EntityFrameworkCore.Audit.Internal.Models;

/// <summary>
/// Represents audit change information for an <see cref="EntityEntry"/>.
/// </summary>
internal sealed class AuditEntry
{
    /// <summary>
    /// Gets the original <see cref="EntityEntry"/>.
    /// </summary>
    public EntityEntry EntityEntry { get; }

    public string? Schema { get; }

    public string? Table { get; }

    /// <summary>
    /// Gets or sets the primary key of the <see cref="EntityEntry"/>.
    /// </summary>
    public string PrimaryKey => EntityEntry.GetPrimaryKey();

    /// <summary>
    /// Gets or sets the type of the <see cref="EntityEntry"/>.
    /// </summary>
    public string EntityType { get; set; } = null!;

    /// <summary>
    /// Gets or sets the changed type of the <see cref="EntityEntry"/>.
    /// </summary>
    public EventType EventType { get; set; }

    /// <summary>
    /// Gets or sets all changed properties information for the <see cref="EntityEntry"/>.
    /// </summary>
    public List<AuditPropertyEntry> Properties { get; set; } = new();

    public AuditEntry(EntityEntry entry)
    {
        EntityEntry = entry;
        Schema = entry.Metadata.GetSchema();
        Table = entry.Metadata.GetTableName();
        EntityType = entry.Metadata.DisplayName();
    }
}
