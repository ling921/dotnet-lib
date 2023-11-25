namespace Ling.EntityFrameworkCore.Audit.Internal.Models;

internal sealed class AuditMetadata
{
    public AuditOperate AllowAnonymousOperate { get; set; }
    public Type? UserKeyType { get; set; }
    public bool HasCreationTime { get; set; }
    public bool HasCreatorId { get; set; }
    public bool HasLastModificationTime { get; set; }
    public bool HasLastModifierId { get; set; }
    public bool HasIsDeleted { get; set; }
    public bool HasDeletionTime { get; set; }
    public bool HasDeleterId { get; set; }

    public void Append(AuditMetadata other)
    {
        if (!ReferenceEquals(this, other))
        {
            AllowAnonymousOperate |= other.AllowAnonymousOperate;
            UserKeyType ??= other.UserKeyType;
            HasCreationTime = HasCreationTime || other.HasCreationTime;
            HasCreatorId = HasCreatorId || other.HasCreatorId;
            HasLastModificationTime = HasLastModificationTime || other.HasLastModificationTime;
            HasLastModifierId = HasLastModifierId || other.HasLastModifierId;
            HasIsDeleted = HasIsDeleted || other.HasIsDeleted;
            HasDeletionTime = HasDeletionTime || other.HasDeletionTime;
            HasDeleterId = HasDeleterId || other.HasDeleterId;
        }
    }

    public bool IsAllowedAnonymousOperate(AuditOperate operate)
    {
        return AllowAnonymousOperate.HasFlag(operate);
    }
}
