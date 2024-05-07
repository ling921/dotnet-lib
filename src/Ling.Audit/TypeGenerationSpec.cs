using Ling.Audit.Extensions;
using Microsoft.CodeAnalysis;
using System.Diagnostics;
using System.Text;

namespace Ling.Audit.SourceGeneration;

[DebuggerDisplay("Type={Type}, ClassType={ClassType}")]
internal class TypeGenerationSpec
{
    public Location Location { get; set; } = null!;

    public Type Type { get; set; } = null!;

    public string ClassDeclaration { get; set; } = null!;

    public List<PropertyGenerationSpec> PropertyGenSpecList { get; set; } = null!;

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append(@$"
{ClassDeclaration}
{{");

        var first = true;
        foreach (var propertyGenSpec in PropertyGenSpecList.OrderBy(p => p.Order))
        {
            if (!first)
            {
                sb.AppendLine();
            }
            else
            {
                first = false;
            }

            propertyGenSpec.IsVirtual = !ClassDeclaration.Contains("sealed");
            sb.Append(propertyGenSpec.ToString().Indent(1));
        }

        sb.Append(@"
}");

        return sb.ToString();
    }
}
