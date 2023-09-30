using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Diagnostics.CodeAnalysis;

namespace Bryan.Swagger;

[ExcludeFromCodeCoverage]
public static class SwaggerExtensions
{
    public static void RegisterFiltersAndAddSecurityDefinition(this SwaggerGenOptions options)
    {
        options.OperationFilter<SecurityRequirementsOperationFilter>(Array.Empty<object>());
        options.DocumentFilter<SwaggerDocumentFilter>(Array.Empty<object>());
        options.DocumentFilter<SwaggerAddEnumDescriptions>(Array.Empty<object>());
        options.AddSecurityDefinition(SecurityRequirementsOperationFilter.OAuthScheme.Scheme, SecurityRequirementsOperationFilter.OAuthScheme);
    }
}
