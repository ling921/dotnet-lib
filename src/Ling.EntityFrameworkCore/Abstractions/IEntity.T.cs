using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LingDev.EntityFrameworkCore.Abstractions;

/// <summary>
/// Represents an entity with a single primary key with "Id" property.
/// </summary>
/// <typeparam name="TKey">The type of primary key.</typeparam>
public interface IEntity<TKey> : IEntity where TKey : notnull
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    [Key]
    [Column(Order = 0)]
    [Comment("The unique identifier.")]
    TKey Id { get; set; }
}
