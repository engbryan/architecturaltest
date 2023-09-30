using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace Bryan.DotNet.OpenTelemetry.Extensions;
[ExcludeFromCodeCoverage]
internal class OpenTelemetryInterceptorHandler : HttpClientHandler
{
    private const string SEND_TEMPLATE = "Fazendo request para: {0}";

    private const string RECEIVE_TEMPLATE = "Recebido response de {0} - {1}";

    private readonly ILogger<OpenTelemetryInterceptorHandler> _logger;

    internal OpenTelemetryInterceptorHandler(ILogger<OpenTelemetryInterceptorHandler> logger)
    {
        _logger = logger;
        ServerCertificateCustomValidationCallback = DangerousAcceptAnyServerCertificateValidator;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug(SEND_TEMPLATE, request.RequestUri);
        }

        var httpResponseMessage = await base.SendAsync(request, cancellationToken);
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug(RECEIVE_TEMPLATE, request.RequestUri, httpResponseMessage.StatusCode);
        }

        return httpResponseMessage;
    }
}
