namespace Ling.Audit;

/// <summary>
/// Indicates that the IsDeleted, DeletionTime and DeleterId property is included.
/// </summary>
/// <typeparam name="TUserKey">The type of user identity.</typeparam>
public interface IDeletionAudited<TUserKey> : ISoftDelete, IHasDeletionTime, IHasDeleter<TUserKey>
    where TUserKey : notnull
{
}
