using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Bryan.Serilog;

[ExcludeFromCodeCoverage]

public static partial class SetupSerilogExtensions
{
    public static LoggingLevelSwitch UseBryanSerilog(this ConfigureHostBuilder host, LoggingLevelSwitch _logLevelSwitcher)
    {
        host.UseSerilog((host, cfg) =>
        {
            var minimumLevel = Enum.TryParse(host.Configuration["Serilog:MinimumLevel"],ignoreCase: true, out LogEventLevel result) ? result : LogEventLevel.Warning;
            _logLevelSwitcher.MinimumLevel = minimumLevel;
            cfg.ReadFrom.Configuration(host.Configuration);
            //cfg.WriteTo.Console(new JsonLogSanitizingFormatter(new JsonFormatter(), true));
            cfg.MinimumLevel.ControlledBy(_logLevelSwitcher)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                .MinimumLevel.Override("HealthChecks", LogEventLevel.Error)
                .MinimumLevel.Override("MicroElements.Swashbuckle.FluentValidation", LogEventLevel.Error)
                .MinimumLevel.Override("System.Net.Http.HttpClient.health-checks", LogEventLevel.Error
            );
        });

        return _logLevelSwitcher;
    }

    public static void MapLogEndpoints(this WebApplication app)
    {
        app.MapGet("api/test-log-level", (ILogger<string> logger, string data) =>
        {
            logger.LogTrace("Message logged: {data}", data);
            logger.LogDebug("Message logged: {data}", data);
            logger.LogInformation("Message logged: {data}", data);
            logger.LogError("Message logged: {data}", data);
            logger.LogCritical("Message logged: {data}", data);
            return Results.Ok(data);
        }).WithTags("Log");

        app.MapGet("api/change-level", (HttpRequest request, [FromServices] LoggingLevelSwitch loggingLevelSwitch, string logLevel) =>
        {
            if (!Enum.TryParse<LogEventLevel>(logLevel, out var result))
            {
                return Results.Ok("Could not convert the queryString logLevel value");
            }

            loggingLevelSwitch.MinimumLevel = result;
            var defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(11, 1);
            defaultInterpolatedStringHandler.AppendLiteral("Changed to ");
            defaultInterpolatedStringHandler.AppendFormatted(result);
            return Results.Ok(defaultInterpolatedStringHandler.ToStringAndClear());
        }).WithTags("Log");
    }
}