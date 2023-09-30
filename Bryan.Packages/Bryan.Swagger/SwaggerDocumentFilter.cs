using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Bryan.Swagger;
[ExcludeFromCodeCoverage]
public class SwaggerDocumentFilter : IDocumentFilter
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    private readonly IWebHostEnvironment _environment;

    public SwaggerDocumentFilter(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment environment)
    {
        _httpContextAccessor = httpContextAccessor;
        _environment = environment;
    }

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var request = _httpContextAccessor.HttpContext!.Request;
        if (_environment.IsDevelopment() && request.Host.Host == "localhost")
        {
            swaggerDoc.Servers.Add(new OpenApiServer
            {
                Url = request.Scheme + "://" + request.Host.Value
            });
        }

        if (request.Headers.TryGetValue("X-Fowarded-Url", out var value) && Uri.IsWellFormedUriString(value[0], UriKind.RelativeOrAbsolute))
        {
            swaggerDoc.Servers.Add(new OpenApiServer
            {
                Url = value[0]
            });
        }
        else
        {
            swaggerDoc.Servers.Add(new OpenApiServer
            {
                Url = "https://" + request.Host.Value
            });
        }
    }
}