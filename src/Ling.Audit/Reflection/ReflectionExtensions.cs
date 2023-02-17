using Microsoft.CodeAnalysis;
using System.Diagnostics;
using System.Reflection;

namespace Ling.Reflection;

internal static partial class ReflectionExtensions
{
    private static bool OpenGenericTypesHaveSamePrefix(Type t1, Type t2)
        => t1.FullName == GetBaseNameFromGenericTypeDef(t2);

    private static string GetBaseNameFromGenericType(Type genericType, bool sourceGenType)
    {
        Type genericTypeDef = genericType.GetGenericTypeDefinition();
        return sourceGenType ? GetBaseNameFromGenericTypeDef(genericTypeDef) : genericTypeDef.FullName!;
    }

    private static string GetBaseNameFromGenericTypeDef(Type genericTypeDef)
    {
        Debug.Assert(genericTypeDef.IsGenericType);
        string fullName = genericTypeDef.FullName!;
        int length = fullName.IndexOf("`") + 2;
        return fullName.Substring(0, length);
    }

    public static CustomAttributeData GetCustomAttributeData(this MemberInfo memberInfo, Type type)
    {
        return memberInfo.CustomAttributes.FirstOrDefault(a => type.IsAssignableFrom(a.AttributeType));
    }

    public static TValue GetConstructorArgument<TValue>(this CustomAttributeData customAttributeData, int index)
    {
        return index < customAttributeData.ConstructorArguments.Count ? (TValue)customAttributeData.ConstructorArguments[index].Value! : default!;
    }

    public static bool IsInitOnly(this MethodInfo method)
    {
        if (method == null)
        {
            throw new ArgumentNullException(nameof(method));
        }

        MethodInfoWrapper methodInfoWrapper = (MethodInfoWrapper)method;
        return methodInfoWrapper.IsInitOnly;
    }

    public static Location? GetDiagnosticLocation(this Type type)
    {
        Debug.Assert(type is TypeWrapper);
        return ((TypeWrapper)type).Location;
    }

    public static Location? GetDiagnosticLocation(this PropertyInfo propertyInfo)
    {
        Debug.Assert(propertyInfo is PropertyInfoWrapper);
        return ((PropertyInfoWrapper)propertyInfo).Location;
    }

    public static Location? GetDiagnosticLocation(this FieldInfo fieldInfo)
    {
        Debug.Assert(fieldInfo is FieldInfoWrapper);
        return ((FieldInfoWrapper)fieldInfo).Location;
    }

    public static Type? GetCompatibleGenericBaseClass(
        this Type type,
        Type baseType,
        Type? objectType = null,
        bool sourceGenType = false)
    {
        // Debug.Assert(baseType.IsGenericType); Debug.Assert(!baseType.IsInterface); Debug.Assert(baseType == baseType.GetGenericTypeDefinition());

        // Work around not being able to use typeof(object) directly during compile-time src gen type analysis.
        objectType ??= typeof(object);

        Type? baseTypeToCheck = type;

        while (baseTypeToCheck != null && baseTypeToCheck != typeof(object))
        {
            if (baseTypeToCheck.IsGenericType)
            {
                Type genericTypeToCheck = baseTypeToCheck.GetGenericTypeDefinition();
                if (genericTypeToCheck == baseType ||
                    (sourceGenType && (OpenGenericTypesHaveSamePrefix(baseType, genericTypeToCheck))))
                {
                    return baseTypeToCheck;
                }
            }

            baseTypeToCheck = baseTypeToCheck.BaseType;
        }

        return null;
    }

    public static bool IsVirtual(this PropertyInfo? propertyInfo)
    {
        Debug.Assert(propertyInfo != null);
        return propertyInfo != null && (propertyInfo.GetMethod?.IsVirtual == true || propertyInfo.SetMethod?.IsVirtual == true);
    }

    public static bool IsKeyValuePair(this Type type, Type? keyValuePairType = null)
    {
        if (!type.IsGenericType)
        {
            return false;
        }

        // Work around not being able to use typeof(KeyValuePair<,>) directly during compile-time src gen type analysis.
        keyValuePairType ??= typeof(KeyValuePair<,>);

        Type generic = type.GetGenericTypeDefinition();
        return generic == keyValuePairType;
    }

    public static object? GetDefaultValue(this ParameterInfo parameterInfo)
    {
        object? defaultValue = parameterInfo.DefaultValue;

        // DBNull.Value is sometimes used as the default value (returned by reflection) of nullable params in place
        // of null.
        if (defaultValue == DBNull.Value && parameterInfo.ParameterType != typeof(DBNull))
        {
            return null;
        }

        return defaultValue;
    }
}
