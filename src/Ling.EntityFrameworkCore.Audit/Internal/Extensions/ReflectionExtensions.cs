namespace Ling.EntityFrameworkCore.Audit.Extensions;

internal static class ReflectionExtensions
{
    internal static string GetFriendlyName(this Type type)
    {
        var friendlyName = type.Name;
        if (type.IsGenericType)
        {
            var iBacktick = friendlyName.IndexOf('`');
            if (iBacktick > 0)
            {
                friendlyName = friendlyName.Remove(iBacktick);
            }
            friendlyName += "<";
            var typeParameters = type.GetGenericArguments();
            for (var i = 0; i < typeParameters.Length; ++i)
            {
                var typeParamName = typeParameters[i].GetFriendlyName();
                friendlyName += i == 0 ? typeParamName : "," + typeParamName;
            }
            friendlyName += ">";
        }

        return friendlyName;
    }
}
