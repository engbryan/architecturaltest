using Dynatrace.OpenTelemetry.Exporter.Metrics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Instrumentation.Http;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics.CodeAnalysis;

namespace Bryan.DotNet.OpenTelemetry.Extensions;
[ExcludeFromCodeCoverage]
public static class DatadogTelemetry
{
    public static IServiceCollection AddDatadogTelemetry(
        this IServiceCollection services,
        string appName,
        string appVersion,
        string datadogServer,
        string datadogApiToken,
        IEnumerable<KeyValuePair<string, object>>? attributes = null,
        Action<HttpClientInstrumentationOptions>? configureHttpClientInstrumentationOptions = null,
        Action<AspNetCoreInstrumentationOptions>? configureAspNetCoreInstrumentationOptions = null,
        ICollection<string>? metrics = null
    )
    {
        services.AddSingleton((IServiceProvider s) => new OpenTelemetryInterceptorHandler(s.GetRequiredService<ILogger<OpenTelemetryInterceptorHandler>>()));
        services.AddHttpClient("OtlpTraceExporter", delegate (HttpClient client)
            {
                client.DefaultRequestHeaders.Add("Authorization", "Api-Token " + datadogApiToken);
            })
            .ConfigurePrimaryHttpMessageHandler<OpenTelemetryInterceptorHandler>();

        services.AddOpenTelemetryMetrics(cfg =>
        {
            cfg.AddMeter(appName, appVersion);
            metrics?.ToList().ForEach(item => cfg.AddMeter(item, appVersion));
            cfg.AddDynatraceExporter(cfg =>
            {
                cfg.EnrichWithDynatraceMetadata = true;
                cfg.Url = datadogServer + "/api/v2/metrics/ingest";
                cfg.ApiToken = datadogApiToken;
            });
        });

        var resourceBuilder = ResourceBuilder.CreateDefault()
            .AddService(appName, appName, appVersion)
            .AddAttributes(attributes)
            .AddTelemetrySdk();

        services.AddSingleton(TracerProvider.Default.GetTracer(appName));
        return services.AddOpenTelemetryTracing(cfg => cfg
                .AddOtlpExporter(exporter =>
                {
                    exporter.Protocol = OtlpExportProtocol.HttpProtobuf;
                    exporter.Endpoint = new Uri(datadogServer + "/v0.3/traces");
                })
                .AddSource(appName)
                .SetResourceBuilder(resourceBuilder)
                .SetErrorStatusOnException()
                .AddHttpClientInstrumentation(opt =>
                {
                    configureHttpClientInstrumentationOptions?.Invoke(opt);
                    opt.RecordException = true;
                })
                .AddAspNetCoreInstrumentation(opt =>
                {
                    configureAspNetCoreInstrumentationOptions?.Invoke(opt);
                    opt.RecordException = true;
                })
                .AddEntityFrameworkCoreInstrumentation(opt =>
                {
                    var setDbStatementForText = opt.SetDbStatementForStoredProcedure = true;
                    opt.SetDbStatementForText = setDbStatementForText;
                }));
    }
}
