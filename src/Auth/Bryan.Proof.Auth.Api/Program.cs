using Bryan.AspNet.Middlewares;
using Bryan.DotNet.OpenTelemetry.Extensions;
using Bryan.Proof.Auth.Api.Configuration;
using Bryan.Proof.Auth.Domain.Repositories;
using Bryan.Serilog;
using Bryan.Swagger;
using Bryan.TokenAuth.Implementations;
using Bryan.TokenAuth.IoC;
using FluentValidation;
using HealthChecks.UI.Client;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Prometheus;
using Prometheus.DotNetRuntime;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Text.Json.Serialization;

CreateCollector();
var builder = WebApplication.CreateBuilder(args);
ConfigureServices(builder);
var app = builder.Build();
Configure(app);
app.Run();

static void CreateCollector()
{
    _ = DotNetRuntimeStatsBuilder.Default();
    var builder = DotNetRuntimeStatsBuilder.Customize()
        .WithContentionStats(CaptureLevel.Informational)
        .WithGcStats(CaptureLevel.Verbose)
        .WithThreadPoolStats(CaptureLevel.Informational)
        .WithExceptionStats(CaptureLevel.Errors)
        .WithJitStats();

    builder.RecycleCollectorsEvery(TimeSpan.FromMinutes(20));
    //builder.StartCollecting();
}

static void ConfigureServices(WebApplicationBuilder builder)
{
    builder.WebHost.ConfigureAppConfiguration(p => AppServiceCollectionExtensions.BuildConfiguration());

    builder.Services.AddSingleton(builder.Host.UseBryanSerilog(new LoggingLevelSwitch()));

    var configuration = builder.Configuration;
    var services = builder.Services;

    services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options
        => options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);

    services.AddEndpointsApiExplorer();
    services.AddValidatorsFromAssemblyContaining<DomainEntryPoint>();
    services.AddFluentValidationRulesToSwagger();

    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = $"{AppServiceCollectionExtensions.APP_NAME} Swagger Docs",
            Version = "v1",
            Description = $"<br>Repositório: <a href='https://git.com' target='_blank'>{AppServiceCollectionExtensions.APP_NAME}</a>",
        });
        c.RegisterFiltersAndAddSecurityDefinition();
    });

    services.AddHttpContextAccessor();
    services.AddHealthChecks()
        .AddCheck("self", () => HealthCheckResult.Healthy(), new[] { "api" })
    //.AddDbContextCheck<DbContext>("database", tags: new[] { "database" })
    ;
    //services
    //    .AddHealthChecksUI(x => x.AddHealthCheckEndpoint("SELF", $"{configuration["urls"].Replace("*", "localhost", StringComparison.OrdinalIgnoreCase)}/hc"))
    //    .AddInMemoryStorage();

    if (!builder.Environment.IsProduction())
        AddTelemetry(services, configuration);

    services.AddBryanJwtAuthentication(configuration, SecurityRequirementsOperationFilter.OAuthScheme.Name);

    services.ConfigureAppDependencies(configuration);
}

static void AddTelemetry(IServiceCollection services, IConfiguration configuration)
{
    var otAddress = configuration.GetValue<string>("OpenTelemetry:ADDRESS");
    var otToken = configuration.GetValue<string>("OpenTelemetry:API_TOKEN");

    if (!string.IsNullOrEmpty(otAddress + otToken))
        services.AddDatadogTelemetry(AppServiceCollectionExtensions.APP_NAME, "1.0", otAddress, otToken, null, ConfigureHttpClient, ConfigureAspNet);

    static void ConfigureHttpClient(OpenTelemetry.Instrumentation.Http.HttpClientInstrumentationOptions options)
    {
        options.RecordException = true;
        options.Filter = ctx => ctx.RequestUri?.ToString() is string url && !url.Contains("logging.awsapis") && !url.Contains("localhost") && !url.Contains("swagger");
        options.Enrich = (activity, _, ctx) =>
        {
            if (string.IsNullOrWhiteSpace(activity.ParentId))
                activity.Stop();

            var context = ctx switch
            {
                HttpRequest req => req.HttpContext,
                HttpResponse resp => resp.HttpContext,
                _ => null
            };
            if (context is not null)
                activity.SetTag("correlation.id", context.Request.Headers[CorrelationIdMiddleware.HEADER_NAME][0]);
        };
    }

    static void ConfigureAspNet(OpenTelemetry.Instrumentation.AspNetCore.AspNetCoreInstrumentationOptions options)
    {
        options.RecordException = true;
        options.Enrich = (activity, _, ctx) =>
        {
            var context = ctx switch
            {
                HttpRequest req => req.HttpContext,
                HttpResponse resp => resp.HttpContext,
                _ => null
            };

            if (context is not null && context.Request.Headers.TryGetValue("X-Fowarded-Url", out var values))
            {
                activity.SetTag("correlation.id", context.Request.Headers[CorrelationIdMiddleware.HEADER_NAME][0]);
                activity.SetTag("http.url", values[0]);
                activity.SetTag("http.host", new Uri(values[0]).Host);
            }
        };
        options.Filter = (ctx) => ctx.Request.GetDisplayUrl().ToString() is string url && !url.Contains("localhost") && !url.Contains("swagger");
    }
}

static void Configure(WebApplication app)
{
    if (app.Environment.IsDevelopment())
        app.UseDeveloperExceptionPage();

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.DocumentTitle = $"{AppServiceCollectionExtensions.APP_NAME} Swagger Docs";
        c.DisplayRequestDuration();
        c.DocExpansion(DocExpansion.None);
        c.EnableDeepLinking();
        c.ShowExtensions();
        c.ShowCommonExtensions();
    });

    app.UseHttpMetrics();
    app.UseMetricServer();
    app.UseCorrelationId();
    app.UseHeaderPropagation();
    app.UseSerilogRequestLogging(opt => opt.GetLevel = (ctx, _, _) => ctx.Request.Path is { Value: "/hc" or "/liveness" } ? LogEventLevel.Verbose : LogEventLevel.Information);

    app.MapHealthChecks("/hc", new() { Predicate = _ => true, ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse });
    app.MapHealthChecks("/liveness", new() { Predicate = r => r.Name.Contains("self", StringComparison.OrdinalIgnoreCase) });
    app.MapHealthChecksUI(x => x.AsideMenuOpened = false);

    //app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapLogEndpoints();
    app.MapEndPoints();
}