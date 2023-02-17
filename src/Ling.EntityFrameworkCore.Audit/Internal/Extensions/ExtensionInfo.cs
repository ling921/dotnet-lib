using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Ling.EntityFrameworkCore.Audit.Internal.Extensions;

internal sealed class ExtensionInfo : DbContextOptionsExtensionInfo
{
    public override bool IsDatabaseProvider { get; }
    public override string LogFragment { get; } = string.Empty;

    public ExtensionInfo(IDbContextOptionsExtension extension) : base(extension)
    {
    }

    public override int GetServiceProviderHashCode() => Extension.GetHashCode();

    public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other) => other is ExtensionInfo;

    public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
    {
    }
}
