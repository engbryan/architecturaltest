using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Bryan.Swagger;
[ExcludeFromCodeCoverage]
public class SwaggerAddEnumDescriptions : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        foreach (var item in swaggerDoc.Components.Schemas.Where(delegate (KeyValuePair<string, OpenApiSchema> x)
        {
            var value = x.Value;
            return value != null && value.Enum?.Count > 0;
        }))
        {
            var @enum = item.Value.Enum;
            if (@enum != null && @enum.Count > 0)
            {
                item.Value.Description = DescribeEnum(@enum, item.Key);
            }
        }

        foreach (var value2 in swaggerDoc.Paths.Values)
        {
            DescribeEnumParameters(value2.Operations, swaggerDoc);
        }
    }

    private static void DescribeEnumParameters(IDictionary<OperationType, OpenApiOperation> operations, OpenApiDocument swaggerDoc)
    {
        if (operations == null)
        {
            return;
        }

        foreach (var param in operations.SelectMany<KeyValuePair<OperationType, OpenApiOperation>, OpenApiParameter>((KeyValuePair<OperationType, OpenApiOperation> x) => x.Value.Parameters))
        {
            var keyValuePair = swaggerDoc.Components.Schemas.FirstOrDefault((KeyValuePair<string, OpenApiSchema> x) => x.Key == param.Name);
            if (keyValuePair.Value != null)
            {
                param.Description = DescribeEnum(keyValuePair.Value.Enum, keyValuePair.Key);
            }
        }
    }

    private static string DescribeEnum(IList<IOpenApiAny> enums, string proprtyTypeName)
    {
        var enumType = GetEnumTypeByName(proprtyTypeName);
        if (enumType is not null)
        {
            return string.Join(", ", enums.OfType<OpenApiInteger>().Select(delegate (OpenApiInteger x)
            {
                var defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 2);
                defaultInterpolatedStringHandler.AppendFormatted(x.Value);
                defaultInterpolatedStringHandler.AppendLiteral(" = ");
                defaultInterpolatedStringHandler.AppendFormatted(Enum.GetName(enumType, x.Value));
                return defaultInterpolatedStringHandler.ToStringAndClear();
            }));
        }

        return "";
    }

    private static Type GetEnumTypeByName(string enumTypeName)
    {
        var enumTypeName2 = enumTypeName;
        return AppDomain.CurrentDomain.GetAssemblies().SelectMany((Assembly x) => x.GetTypes()).First((Type x) => x.Name == enumTypeName2);
    }
}
