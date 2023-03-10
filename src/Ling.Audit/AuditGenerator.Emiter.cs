using Ling.EntityFrameworkCore.Audit.Generator;
using Ling.SourceGeneration;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace Ling.Audit.SourceGeneration;

public sealed partial class AuditGenerator
{
    private sealed class Emiter
    {
        private readonly AuditSourceGenerationContext _sourceGenerationContext;
        private readonly SourceGenerationSpec _generationSpec;

        private static readonly DiagnosticDescriptor UserKeyTypeConflict = new(
            id: "LDEFCG003",
            title: GeneratorConstants.ConflictUserKeyTypeNameTitle,
            messageFormat: GeneratorConstants.ConflictUserKeyTypeNameMessageFormat,
            category: GeneratorConstants.AuditSourceGenerationName,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        private static readonly DiagnosticDescriptor DuplicateSourceName = new(
            id: "LDEFCG004",
            title: GeneratorConstants.DuplicateSourceNameTitle,
            messageFormat: GeneratorConstants.DuplicateSourceNameMessageFormat,
            category: GeneratorConstants.AuditSourceGenerationName,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        internal Emiter(in AuditSourceGenerationContext sourceGenerationContext, SourceGenerationSpec generationSpec)
        {
            _sourceGenerationContext = sourceGenerationContext;
            _generationSpec = generationSpec;
        }

        internal void Emit()
        {
            var hintNames = new HashSet<string>();
            foreach (var contextGenerationSpec in _generationSpec.ContextGenerationSpecList)
            {
                if (contextGenerationSpec is null)
                {
                    continue;
                }

                var hintName = $"{contextGenerationSpec.ContextType.Name}.g.cs";
                if (hintNames.Contains(hintName))
                {
                    _sourceGenerationContext.ReportDiagnostic(Diagnostic.Create(DuplicateSourceName, contextGenerationSpec.Location, hintName));
                    continue;
                }

                var userKeyPropertyGroup = contextGenerationSpec.GenerationType.PropertyGenSpecList
                    .Where(p => p.IsUerKeyType)
                    .OrderBy(p => p.Order)
                    .GroupBy(p => p.DeclaringTypeRef);
                if (userKeyPropertyGroup.Count() > 1)
                {
                    _sourceGenerationContext.ReportDiagnostic(Diagnostic.Create(UserKeyTypeConflict, userKeyPropertyGroup.ElementAt(1).First().Location));
                }

                var sb = new StringBuilder(@"// <auto-generated/>

#nullable enable annotations
#nullable disable warnings");

                sb.Append(@$"

namespace {contextGenerationSpec.ContextType.Namespace}
{{");
                var declarationCount = contextGenerationSpec.ContextClassDeclarationList.Count;
                var i = 1;
                for (; i < declarationCount; i++)
                {
                    var declarationSource = $@"
{contextGenerationSpec.ContextClassDeclarationList[declarationCount - i]}
{{";
                    sb.Append(declarationSource.Indent(i));
                }

                sb.Append(contextGenerationSpec.GenerationType.ToString().Indent(i));

                const string ClosingBraces = @"
}";
                while (i > 1)
                {
                    sb.Append(ClosingBraces.Indent(--i));
                }

                sb.AppendLine(ClosingBraces);

                _sourceGenerationContext.AddSource(hintName, SourceText.From(sb.ToString(), Encoding.UTF8));
                hintNames.Add(hintName);
            }
        }
    }
}
