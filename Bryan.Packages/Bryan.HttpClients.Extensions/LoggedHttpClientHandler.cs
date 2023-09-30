using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Bryan.HttpClients.Extensions;

[ExcludeFromCodeCoverage]
public class LoggedHttpClientHandler : LoggedHttpClientHandler<LoggedHttpClientHandler, LoggedHttpClientHandler>
{
    public LoggedHttpClientHandler(ILogger<LoggedHttpClientHandler<LoggedHttpClientHandler, LoggedHttpClientHandler>> logger) : base(logger)
    {
    }
}

[ExcludeFromCodeCoverage]
public class LoggedHttpClientHandler<TRequest, TResponse> : DelegatingHandler
{
    private const string REQUEST = "Requesting api {verb} => {url}";
    private const string REQUEST_DEBUG = "Requesting api {verb} => {url} with data: {data}";
    private const string RESPONSE = "Received statusCode {statusCode} from {url} in {duration}";
    private const string RESPONSE_DEBUG = "Received statusCode {statusCode} from {url} in {duration} with data: {data}";

    private readonly ILogger<LoggedHttpClientHandler<TRequest, TResponse>> _logger;

    public LoggedHttpClientHandler(ILogger<LoggedHttpClientHandler<TRequest, TResponse>> logger) => _logger = logger;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var requestUri = request.RequestUri!.ToString();
        if (_logger.IsEnabled(LogLevel.Debug) && request.Content != null)
        {
            var text = await request.Content!.ReadAsStringAsync(cancellationToken);
            if (typeof(TRequest) == typeof(LoggedHttpClientHandler))
                _logger.LogDebug(REQUEST_DEBUG, request.Method, requestUri, text);
            else
                _logger.LogDebug(REQUEST_DEBUG, request.Method, requestUri, JsonSerializer.Deserialize<TRequest>(text));
        }
        else
        {
            _logger.LogInformation(REQUEST, request.Method, requestUri);
        }

        var stopwatch = Stopwatch.StartNew();
        var response = await base.SendAsync(request, cancellationToken);
        stopwatch.Stop();
        var level = response.IsSuccessStatusCode ? LogLevel.Information : LogLevel.Warning;
        string message;
        object[] args;
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            var text = await response.Content.ReadAsStringAsync(cancellationToken);
            if (typeof(TRequest) == typeof(LoggedHttpClientHandler))
                args = new object[4] { (int)response.StatusCode, requestUri, stopwatch.Elapsed, text };
            else
                args = new object[4] { (int)response.StatusCode, requestUri, stopwatch.Elapsed, JsonSerializer.Deserialize<TResponse>(text)! };
            message = RESPONSE_DEBUG;
        }
        else
        {
            args = new object[3] { (int)response.StatusCode, requestUri, stopwatch.Elapsed };
            message = RESPONSE;
        }

        _logger.Log(level, message, args);
        return response;
    }
}