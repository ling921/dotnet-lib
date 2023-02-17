#if NET7_0_OR_GREATER
using System.ComponentModel;
#endif

namespace Ling.EntityFrameworkCore.Audit;

/// <summary>
/// Interface for audit user information.
/// </summary>
public interface IAuditUserProvider
{
    /// <summary>
    /// Gets the identity of user.
    /// </summary>
    public string? Identity { get; }

    /// <summary>
    /// Gets the name of user.
    /// </summary>
    public string? Name { get; }

    /// <summary>
    /// Get the user identity match specific type.
    /// </summary>
    /// <param name="type">The type to get.</param>
    /// <returns>The user identity.</returns>
#if NET7_0_OR_GREATER
    public virtual object? GeIdentityOfType(Type type)
    {
        if (string.IsNullOrWhiteSpace(Identity) || type is null)
        {
            return null;
        }

        try
        {
            var underlyingType = Nullable.GetUnderlyingType(type) ?? type;
            if (type.IsPrimitive)
            {
                return Convert.ChangeType(Identity, underlyingType);
            }

            var method = underlyingType.GetMethod("Parse", new Type[] { typeof(string) });
            if (method is not null)
            {
                return method.Invoke(null, new object[] { Identity });
            }

            var converter = TypeDescriptor.GetConverter(underlyingType);
            if (converter is not null)
            {
                return converter.ConvertFrom(Identity);
            }
        }
        catch { }

        return null;
    }
#else
    object? GeIdentityOfType(Type type);
#endif
}
