namespace Ling.Audit.SourceGeneration;

internal static class AuditDefaults
{
    public const string SourceGenerationName = "Ling.Audit.SourceGeneration";

    public const string CreationTimeClrName = "CreationTime";
    public const string CreatorIdClrName = "CreatorId";
    public const string LastModificationTimeClrName = "LastModificationTime";
    public const string LastModifierIdClrName = "LastModifierId";
    public const string IsDeletedClrName = "IsDeleted";
    public const string DeletionTimeClrName = "DeletionTime";
    public const string DeleterIdClrName = "DeleterId";

    public const string CreationTimeComment = "Gets or sets the creation time of this entity.";
    public const string CreatorIdComment = "Gets or sets the creator Id of this entity.";
    public const string LastModificationTimeComment = "Gets or sets the last modification time of this entity.";
    public const string LastModifierIdComment = "Gets or sets the last modifier Id of this entity.";
    public const string IsDeletedComment = "Gets or sets whether this entity is soft deleted.";
    public const string DeletionTimeComment = "Gets or sets the deletion time of this entity.";
    public const string DeleterIdComment = "Get or set the deleter Id of this entity.";
}
