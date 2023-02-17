using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LingDev.EntityFrameworkCore.Abstractions;

/// <summary>
/// Represents an entity with a single primary key with "Id" property.
/// </summary>
/// <typeparam name="TKey">The type of primary key.</typeparam>
public abstract class Entity<TKey> : IEntity<TKey> where TKey : notnull
{
    /// <inheritdoc/>
    [Key]
    [Column(Order = 0)]
    [Comment("The unique identifier.")]
    public virtual TKey Id { get; set; } = default!;
}
