using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using System.Diagnostics.CodeAnalysis;

namespace Bryan.HttpClients.Extensions;

[ExcludeFromCodeCoverage]
public static class RefitExtensions
{
    public static IHttpClientBuilder ConfigureRefitService<T>(
        this IServiceCollection services,
        string uri,
        string appName,
        Action<RefitSettings>? refitSetup = null,
        bool exporter = false
    )
        where T : class
    {
        if (!services.Any(p => p.ServiceType == typeof(LoggedHttpClientHandler)))
            services.AddTransient<LoggedHttpClientHandler>();
        if (exporter && !services.Any(p => p.ServiceType == typeof(RawHttpClientRequestResponseExporterHandler)))
            services.AddTransient<RawHttpClientRequestResponseExporterHandler>();

        RefitSettings RefitSettings = new();
        refitSetup?.Invoke(RefitSettings);

        RefitSettings.ContentSerializer ??= new SystemTextJsonContentSerializer(new System.Text.Json.JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
        });

        var httpClientBuilder = services
            .AddRefitClient<T>(RefitSettings)
            .AddHttpMessageHandler<LoggedHttpClientHandler>()
            .ConfigureHttpClient(httpClient =>
            {
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(appName);
                httpClient.BaseAddress = new Uri(uri);
            });

        if (exporter)
            httpClientBuilder.AddHttpMessageHandler<RawHttpClientRequestResponseExporterHandler>();

        return httpClientBuilder;
    }
}