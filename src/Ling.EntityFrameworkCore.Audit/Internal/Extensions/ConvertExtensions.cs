using System.ComponentModel;

namespace Ling.EntityFrameworkCore.Audit.Internal.Extensions;

internal static class ConvertExtensions
{
    internal static object? ConvertToTargetType(this object? value, Type? targetType)
    {
        if (targetType is null || value is null)
        {
            return value;
        }

        var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;
        var valueType = value.GetType();
        if (valueType == targetType || valueType == underlyingType)
        {
            return value;
        }

        try
        {
            if (targetType.IsPrimitive)
            {
                return Convert.ChangeType(value, underlyingType);
            }

            var method = underlyingType.GetMethod("Parse", new Type[] { typeof(string) });
            if (method is not null)
            {
                return method.Invoke(null, new object?[] { value?.ToString() });
            }

            var converter = TypeDescriptor.GetConverter(underlyingType);
            if (converter is not null)
            {
                return converter.ConvertFrom(value);
            }
        }
        catch
        {
        }

        return null;
    }
}
