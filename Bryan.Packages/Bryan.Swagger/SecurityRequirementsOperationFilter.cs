using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Diagnostics.CodeAnalysis;

namespace Bryan.Swagger;
[ExcludeFromCodeCoverage]
public class SecurityRequirementsOperationFilter : IOperationFilter
{
    internal const string TOKEN_TYPE = "bearer";
    private const string AUTHORIZATION = "Authorization";
    private const string TOKEN_DESCRIPTION = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter your token in the text input below.\r\n\r\nExample: \"12345abcdef\"";

    public static readonly OpenApiSecurityScheme OAuthScheme = new()
    {
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = TOKEN_TYPE
        },
        Description = TOKEN_DESCRIPTION,
        Name = AUTHORIZATION,
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = TOKEN_TYPE,
        BearerFormat = "JWT"
    };

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var second = from attr in context.MethodInfo.DeclaringType!.GetCustomAttributes(inherit: true).OfType<AuthorizeAttribute>()
            select attr.Policy;
        var source = (from attr in context.MethodInfo.GetCustomAttributes(inherit: true).OfType<AuthorizeAttribute>()
            select attr.Policy).Union<string>(second).Distinct();
        var flag = context.ApiDescription.ActionDescriptor.EndpointMetadata.OfType<AuthorizeAttribute>().Any();
        if (source.Any() || flag)
        {
            operation.Responses.Add("401", new OpenApiResponse
            {
                Description = "Unauthorized"
            });
            operation.Responses.Add("403", new OpenApiResponse
            {
                Description = "Forbidden"
            });
            operation.Security = new OpenApiSecurityRequirement[1]
            {
                new OpenApiSecurityRequirement { [OAuthScheme] = Array.Empty<string>() }
            };
        }
    }
}
