using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace Bryan.AspNet.Middlewares;
public static class CorrelationIdMiddleware
{
    public const string HEADER_NAME = "X-Correlation-Id";

    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app)
        => app.Use(async delegate (HttpContext httpContext, Func<Task> next)
        {
            if (!httpContext.Request.Headers.ContainsKey(HEADER_NAME))
            {
                var text = Guid.NewGuid().ToString();
                httpContext.Request.Headers.Add(HEADER_NAME, text);
                httpContext.Response.Headers.Add(HEADER_NAME, text);
                Activity.Current?.AddTag(HEADER_NAME, text);
            }

            await next();
        });
}
