using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Ling.EntityFrameworkCore.Audit.Extensions;

internal static class EntityEntryExtensions
{
    internal static string GetPrimaryKey(this EntityEntry entityEntry)
    {
        var key = entityEntry.Metadata.FindPrimaryKey();
        return key is null
            ? string.Empty
            : string.Join(",", key.Properties.ToDictionary(x => x.Name, x => x.PropertyInfo?.GetValue(entityEntry.Entity)).Select(x => $"{x.Key}={x.Value}"));
    }
}
