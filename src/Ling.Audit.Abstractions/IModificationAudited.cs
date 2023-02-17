namespace Ling.Audit;

/// <summary>
/// Indicates that the LastModificationTime and LastModifierId property is included.
/// </summary>
/// <typeparam name="TUserKey">The type of user identity.</typeparam>
public interface IModificationAudited<TUserKey> : IHasModificationTime, IHasModifier<TUserKey>
    where TUserKey : notnull
{
}
