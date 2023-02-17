namespace Ling.EntityFrameworkCore.Audit.Internal;

internal static class Constants
{
    public const string Id = "Id";
    public const string CreationTime = "CreationTime";
    public const string CreatorId = "CreatorId";
    public const string LastModificationTime = "LastModificationTime";
    public const string LastModifierId = "LastModifierId";
    public const string IsDeleted = "IsDeleted";
    public const string DeletionTime = "DeletionTime";
    public const string DeleterId = "DeleterId";

    public static readonly IReadOnlyCollection<string> PropertyNames = new[]
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
