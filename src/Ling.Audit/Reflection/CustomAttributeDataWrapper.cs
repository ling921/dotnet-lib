using Microsoft.CodeAnalysis;
using System.Reflection;

namespace Ling.Reflection;

internal class CustomAttributeDataWrapper : CustomAttributeData
{
    public override ConstructorInfo Constructor { get; }

    public override IList<CustomAttributeNamedArgument> NamedArguments { get; }

    public override IList<CustomAttributeTypedArgument> ConstructorArguments { get; }

    public CustomAttributeDataWrapper(AttributeData a, MetadataLoadContextInternal metadataLoadContext)
    {
        var namedArguments = new List<CustomAttributeNamedArgument>();
        foreach (var na in a.NamedArguments)
        {
            var member = a.AttributeClass!.GetMembers(na.Key).First();

            MemberInfo memberInfo = member is IPropertySymbol propertySymbol
                ? new PropertyInfoWrapper(propertySymbol, metadataLoadContext)
                : new FieldInfoWrapper((IFieldSymbol)member, metadataLoadContext);

            namedArguments.Add(new CustomAttributeNamedArgument(memberInfo, na.Value.Value));
        }

        var constructorArguments = new List<CustomAttributeTypedArgument>();

        foreach (var ca in a.ConstructorArguments)
        {
            if (ca.Kind == TypedConstantKind.Error)
            {
                continue;
            }

            var value = ca.Kind == TypedConstantKind.Array ? ca.Values : ca.Value;
            constructorArguments.Add(new CustomAttributeTypedArgument(ca.Type?.AsType(metadataLoadContext), value));
        }

        Constructor = new ConstructorInfoWrapper(a.AttributeConstructor!, metadataLoadContext);
        NamedArguments = namedArguments;
        ConstructorArguments = constructorArguments;
    }
}
