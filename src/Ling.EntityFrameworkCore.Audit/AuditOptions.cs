using System.Security.Claims;

namespace Ling.EntityFrameworkCore.Audit;

/// <summary>
/// Option to configure the behavior of audited entities.
/// </summary>
public class AuditOptions
{
    /// <summary>
    /// Whether to allow anonymous creation of audit entities, default to <see langword="false"/>.
    /// <para>
    /// An <see cref="InvalidOperationException"/> will be thrown when anonymous creation, you can
    /// use <see cref="AuditIncludeAttribute"/> to specify anonymous operations that entities can perform.
    /// </para>
    /// <para>If <see langword="true"/>, it will allow anonymous creation for all audit entities.</para>
    /// </summary>
    public bool AllowAnonymousCreate { get; set; }

    /// <summary>
    /// Whether to allow anonymous modification of audit entities, default to <see langword="false"/>.
    /// <para>
    /// An <see cref="InvalidOperationException"/> will be thrown when anonymous modification, you
    /// can use <see cref="AuditIncludeAttribute"/> to specify anonymous operations that entities can perform.
    /// </para>
    /// <para>If <see langword="true"/>, it will allow anonymous modification for all audit entities.</para>
    /// </summary>
    public bool AllowAnonymousModify { get; set; }

    /// <summary>
    /// Whether to allow anonymous deletion of audit entities, default to <see langword="false"/>.
    /// <para>
    /// An <see cref="InvalidOperationException"/> will be thrown when anonymous deletion, you can
    /// use <see cref="AuditIncludeAttribute"/> to specify anonymous operations that entities can perform.
    /// </para>
    /// <para>If <see langword="true"/>, it will allow anonymous deletion for all audit entities.</para>
    /// </summary>
    public bool AllowAnonymousDelete { get; set; }

    /// <summary>
    /// Claim type of user identity.
    /// </summary>
    public string UserIdClaimType { get; set; } = ClaimTypes.NameIdentifier;

    /// <summary>
    /// Claim type of user name.
    /// </summary>
    public string UserNameClaimType { get; set; } = ClaimTypes.Name;

    /// <summary>
    /// Comments.
    /// </summary>
    public AuditEntityComments Comments { get; set; } = new();
}

/// <summary>
/// Comments to the audited entities.
/// </summary>
public class AuditEntityComments
{
    /// <summary>
    /// Comment to Id property.
    /// </summary>
    public string Id { get; set; } = "Unique identifier, primary key.";

    /// <summary>
    /// Comment to CreationTime property.
    /// </summary>
    public string CreationTime { get; set; } = "The date and time when created this entity.";

    /// <summary>
    /// Comment to CreatorId property.
    /// </summary>
    public string CreatorId { get; set; } = "The primary key of the user who created this entity.";

    /// <summary>
    /// Comment to LastModificationTime property.
    /// </summary>
    public string LastModificationTime { get; set; } = "The primary key of the user who modified this entity.";

    /// <summary>
    /// Comment to LastModifierId property.
    /// </summary>
    public string LastModifierId { get; set; } = "The date and time when modified this entity.";

    /// <summary>
    /// Comment to IsDeleted property.
    /// </summary>
    public string IsDeleted { get; set; } = "A flag indicating if the entity has mark as deleted instead of actually deleting it.";

    /// <summary>
    /// Comment to DeletionTime property.
    /// </summary>
    public string DeletionTime { get; set; } = "The date and time when marked this entity as deleted.";

    /// <summary>
    /// Comment to DeleterId property.
    /// </summary>
    public string DeleterId { get; set; } = "The primary key of the user who marked this entity as deleted.";
}
