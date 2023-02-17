using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Ling.EntityFrameworkCore.Audit.Internal.Models;

/// <summary>
/// Represents the property change information of the <see cref="EntityEntry"/>.
/// </summary>
internal sealed class AuditPropertyEntry
{
    /// <summary>
    /// The property name.
    /// </summary>
    public string PropertyName { get; set; } = null!;

    /// <summary>
    /// The original value of the property.
    /// </summary>
    public object? OriginalValue { get; set; }

    /// <summary>
    /// The new value of the property.
    /// </summary>
    public object? NewValue { get; set; }
}
