using Microsoft.CodeAnalysis;
using System.Diagnostics;

namespace Ling.Audit.SourceGeneration;

[DebuggerDisplay("Name={Name}, Type={TypeMetadata}")]
internal class PropertyGenerationSpec
{
    public Location Location { get; set; } = null!;

    public string ClrName { get; set; } = null!;

    public bool IsVirtual { get; set; } = true;

    public bool IsValueType { get; set; }

    public bool IsNullable { get; set; }

    public bool IsUerKeyType { get; set; }

    public int Order { get; set; }

    public string? Comment { get; set; }

    public Type? ImplementingType { get; set; }

    /// <summary>
    /// Compilable name of the property's declaring type.
    /// </summary>
    public string DeclaringTypeRef { get; set; } = null!;

    public override string ToString()
    {
        var propertyTypeRef = IsValueType && IsNullable
            ? $"global::System.Nullable<{DeclaringTypeRef}>"
            : (IsValueType ? DeclaringTypeRef : $"{DeclaringTypeRef}?");

        return $@"
/// <summary>
/// {Comment}
/// </summary>
public {(IsVirtual ? "virtual " : string.Empty)}{propertyTypeRef} {ClrName} {{ get; set; }}";
    }
}
