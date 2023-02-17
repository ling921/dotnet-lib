using System.Diagnostics;

namespace Ling.Audit.SourceGeneration;

[DebuggerDisplay("ContextTypeRef={ContextTypeRef}")]
internal sealed class SourceGenerationSpec
{
    public List<ContextGenerationSpec> ContextGenerationSpecList { get; }

    public SourceGenerationSpec(List<ContextGenerationSpec> generationContexts)
    {
        ContextGenerationSpecList = generationContexts;
    }
}
