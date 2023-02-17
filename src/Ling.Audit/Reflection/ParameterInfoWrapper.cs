using Microsoft.CodeAnalysis;
using System.Reflection;

namespace Ling.Reflection;

internal class ParameterInfoWrapper : ParameterInfo
{
    private readonly IParameterSymbol _parameter;

    private readonly MetadataLoadContextInternal _metadataLoadContext;

    public override Type? ParameterType => _parameter.Type.AsType(_metadataLoadContext);

    public override string Name => _parameter.Name;

    public override bool HasDefaultValue => _parameter.HasExplicitDefaultValue;

    public override object? DefaultValue => HasDefaultValue ? _parameter.ExplicitDefaultValue : null;

    public override int Position => _parameter.Ordinal;

    public ParameterInfoWrapper(IParameterSymbol parameter, MetadataLoadContextInternal metadataLoadContext)
    {
        _parameter = parameter;
        _metadataLoadContext = metadataLoadContext;
    }

    public override IList<CustomAttributeData> GetCustomAttributesData()
    {
        var attributes = new List<CustomAttributeData>();
        foreach (var a in _parameter.GetAttributes())
        {
            attributes.Add(new CustomAttributeDataWrapper(a, _metadataLoadContext));
        }
        return attributes;
    }
}
