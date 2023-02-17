namespace Ling.Audit;

/// <summary>
/// Indicates that the CreatorId property is included.
/// </summary>
/// <typeparam name="TUserKey">The type of user identity.</typeparam>
public interface IHasCreator<TUserKey> where TUserKey : notnull
{
}
