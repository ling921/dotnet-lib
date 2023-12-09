using System.Diagnostics;

namespace Ling.Audit.Extensions;

internal static class StringHelper
{
    public static string Indent(this string source, int numIndentations, int spacePerIndentation = 4)
    {
        Debug.Assert(numIndentations >= 1);
        Debug.Assert(spacePerIndentation >= 1);

        return source.Replace("\n", "\n" + new string(' ', spacePerIndentation * numIndentations));
    }

    public static string Join<T>(this IEnumerable<T> source, string separator)
    {
        return string.Join(separator, source);
    }
}
