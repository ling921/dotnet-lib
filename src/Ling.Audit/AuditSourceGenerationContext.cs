using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Ling.EntityFrameworkCore.Audit.Generator;

internal class AuditSourceGenerationContext
{
    private readonly SourceProductionContext _context;

    public AuditSourceGenerationContext(SourceProductionContext context)
    {
        _context = context;
    }

    public void ReportDiagnostic(Diagnostic diagnostic)
    {
        _context.ReportDiagnostic(diagnostic);
    }

    public void AddSource(string hintName, SourceText sourceText)
    {
        _context.AddSource(hintName, sourceText);
    }
}
