using Ling.EntityFrameworkCore.Audit.Generator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

namespace Ling.Audit.SourceGeneration;

/// <summary>
/// Source code generator for audit properties.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed partial class AuditGenerator : IIncrementalGenerator
{
    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (s, ct) => Parser.IsSyntaxTargetForGeneration(s, ct),
                static (s, ct) => Parser.GetSemanticTargetForGeneration(s, ct))
            .Where(static c => c is not null)
            .Select(static (c, _) => c!);

        var compilationAndClasses = context.CompilationProvider.Combine(classDeclarations.Collect());
        context.RegisterSourceOutput(compilationAndClasses, (spc, source) => Execute(source.Left, source.Right, spc));
    }

    private void Execute(
        Compilation compilation,
        ImmutableArray<ClassDeclarationSyntax> contextClasses,
        SourceProductionContext sourceProductionContext)
    {
#if LAUNCH_DEBUGGER
        if (!Diagnostics.Debugger.IsAttached)
        {
            Diagnostics.Debugger.Launch();
        }
#endif
        if (contextClasses.IsDefaultOrEmpty)
        {
            return;
        }

        var context = new AuditSourceGenerationContext(sourceProductionContext);
        var parser = new Parser(compilation, context);
        var spec = parser.GetGenerationSpec(contextClasses, sourceProductionContext.CancellationToken);
        if (spec is not null)
        {
            var emitter = new Emiter(context, spec);
            emitter.Emit();
        }
    }
}
