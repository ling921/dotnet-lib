namespace Ling.Audit.SourceGeneration;

internal static class GeneratorConstants
{
    public const string AuditSourceGenerationName = "Ling.EntityFrameworkCore.Audit.SourceGeneration";

    public const string CreationTimeComment = "Gets or sets the creation time of this entity.";
    public const string CreatorIdComment = "Gets or sets the creator Id of this entity.";
    public const string LastModificationTimeComment = "Gets or sets the last modification time of this entity.";
    public const string LastModifierIdComment = "Gets or sets the last modifier Id of this entity.";
    public const string IsDeletedComment = "Gets or sets whether this entity is soft deleted.";
    public const string DeletionTimeComment = "Gets or sets the deletion time of this entity.";
    public const string DeleterIdComment = "Get or set the deleter Id of this entity.";

    public const string ConflictUserKeyTypeNameTitle = "The audit 'TUserKey' type should be consistent.";
    public const string ConflictUserKeyTypeNameMessageFormat = "The audit 'TUserKey' type should be consistent.";
    public const string ContextClassesMustBePartialTitle = "The audit types and all containing types must be partial.";
    public const string ContextClassesMustBePartialMessageFormat = "The type '{0}' and its' all containing types must be made partial to kick off source generation.";
    public const string UnknownUserKeyTypeTitle = "Unable to resolve 'TUserKey' type.";
    public const string UnknownUserKeyTypeMessageFormat = "Unable to resolve 'TUserKey' type for interface '{0}'.";
    public const string DuplicateSourceNameTitle = "A source with the same name already exists.";
    public const string DuplicateSourceNameMessageFormat = "A source with the name '{0}' already exists.";
}
