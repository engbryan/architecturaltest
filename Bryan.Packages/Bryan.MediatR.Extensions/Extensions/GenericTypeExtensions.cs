using System.Diagnostics.CodeAnalysis;

namespace Bryan.MediatR.Extensions;

[ExcludeFromCodeCoverage]
public static class GenericTypeExtensions
{
    public static string GetGenericTypeName(this Type type)
    {
        if (type.IsGenericType)
        {
            var text = string.Join(",", type.GetGenericArguments().Select(p => p.Name).ToArray());
            return type.Name.Remove(type.Name.IndexOf('`')) + "<" + text + ">";
        }

        return type.Name;
    }

    public static string GetGenericTypeName(this object @object) => @object.GetType().GetGenericTypeName();
}