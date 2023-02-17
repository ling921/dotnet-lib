namespace Ling.Audit;

/// <summary>
/// Indicates that the IsDeleted and DeleterId property is included.
/// </summary>
/// <typeparam name="TUserKey">The type of user identity.</typeparam>
public interface IHasDeleter<TUserKey> : ISoftDelete where TUserKey : notnull
{
}
