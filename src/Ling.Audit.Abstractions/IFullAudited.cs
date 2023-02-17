namespace Ling.Audit;

/// <summary>
/// Indicates that the CreationTime, CreatorId, LastModificationTime, LastModifierId, IsDeleted,
/// DeletionTime and DeleterId property is included.
/// </summary>
/// <typeparam name="TUserKey">The type of user identity.</typeparam>
public interface IFullAudited<TUserKey> : ICreationAudited<TUserKey>, IModificationAudited<TUserKey>, IDeletionAudited<TUserKey>
    where TUserKey : notnull
{
}
