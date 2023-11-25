namespace Ling.EntityFrameworkCore.Audit.Internal;

internal static class Constants
{
    internal const string IncludeAnnotationName = "Ling:Audit:Include";
    internal const string MetadataAnnotationName = "Ling:Audit:Metadata";

    internal const string Id = "Id";
    internal const string CreationTime = "CreationTime";
    internal const string CreatorId = "CreatorId";
    internal const string LastModificationTime = "LastModificationTime";
    internal const string LastModifierId = "LastModifierId";
    internal const string IsDeleted = "IsDeleted";
    internal const string DeletionTime = "DeletionTime";
    internal const string DeleterId = "DeleterId";

    internal static readonly IReadOnlyCollection<string> PropertyNames = new[]
    {
        Id,
        CreationTime,
        CreatorId,
        LastModificationTime,
        LastModifierId,
        IsDeleted,
        DeletionTime,
        DeleterId,
    };
}
