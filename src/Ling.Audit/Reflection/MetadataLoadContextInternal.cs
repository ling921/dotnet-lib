using Microsoft.CodeAnalysis;
using System.Diagnostics;

namespace Ling.Reflection;

internal class MetadataLoadContextInternal
{
    private readonly Compilation _compilation;

    public Compilation Compilation => _compilation;

    public MetadataLoadContextInternal(Compilation compilation)
    {
        _compilation = compilation;
    }

    public Type? Resolve(Type type)
    {
        Debug.Assert(!type.IsArray, "Resolution logic only capable of handling named types.");
        return Resolve(type.FullName!);
    }

    public Type? Resolve(string fullyQualifiedMetadataName)
    {
        var typeSymbol = _compilation.GetBestTypeByMetadataName(fullyQualifiedMetadataName);
        return typeSymbol?.AsType(this);
    }

    public Type? Resolve(SpecialType specialType)
    {
        var typeSymbol = _compilation.GetSpecialType(specialType);
        return typeSymbol.AsType(this);
    }
}
