namespace Ling.Audit;

/// <summary>
/// Indicates that the LastModifierId property is included.
/// </summary>
/// <typeparam name="TUserKey">The type of user identity.</typeparam>
public interface IHasModifier<TUserKey> where TUserKey : notnull
{
}
