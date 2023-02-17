namespace Ling.Audit;

/// <summary>
/// Indicates that the CreationTime and CreatorId property is included.
/// </summary>
/// <typeparam name="TUserKey">The type of user identity.</typeparam>
public interface ICreationAudited<TUserKey> : IHasCreationTime, IHasCreator<TUserKey>
    where TUserKey : notnull
{
}
