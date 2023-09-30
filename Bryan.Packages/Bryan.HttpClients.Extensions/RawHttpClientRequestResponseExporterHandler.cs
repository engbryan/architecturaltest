using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Bryan.HttpClients.Extensions;

public interface IRawHttpRequestResponseExporter
{
    Task Export(string rawRequest, string rawResponse, string baseUrl, TimeSpan elapsedTime);
}

[ExcludeFromCodeCoverage]
public sealed class RawHttpClientRequestResponseExporterHandler : DelegatingHandler
{
    private readonly IRawHttpRequestResponseExporter _exporter;

    private readonly ILogger<RawHttpClientRequestResponseExporterHandler> _logger;

    public RawHttpClientRequestResponseExporterHandler(IRawHttpRequestResponseExporter exporter, ILogger<RawHttpClientRequestResponseExporterHandler> logger)
    {
        _exporter = exporter;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var baseUrl = request.RequestUri!.GetLeftPart(UriPartial.Authority).ToString();
        var stopwatch = Stopwatch.StartNew();
        var response = await base.SendAsync(request, cancellationToken);
        stopwatch.Stop();
        try
        {
            var rawRequest = await ToRawString(request);
            var text = await ToRawString(response);
            await _exporter.Export(rawRequest.Trim(), text.Trim(), baseUrl, stopwatch.Elapsed);
            return response;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Problema ao exportar request e response");
            return response;
        }
    }

    private static async Task<string> ToRawString(HttpRequestMessage request)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{request.Method} {request.RequestUri} HTTP/{request.Version}");
        string key;
        IEnumerable<string> value;
        foreach (var header in request.Headers)
        {
            header.Deconstruct(out key, out value);
            var text = key;
            foreach (var item in value)
            {
                sb.AppendLine(text + ": " + item);
            }
        }

        if (request.Content?.Headers != null)
        {
            foreach (var header2 in request.Content!.Headers)
            {
                header2.Deconstruct(out key, out value);
                var text2 = key;
                foreach (var item2 in value)
                {
                    sb.AppendLine(text2 + ": " + item2);
                }
            }
        }

        sb.AppendLine();
        if (request.Content != null)
        {
            var value2 = await request.Content!.ReadAsStringAsync();
            if (!string.IsNullOrWhiteSpace(value2))
            {
                sb.AppendLine(value2);
            }
        }

        return sb.ToString();
    }

    private static async Task<string> ToRawString(HttpResponseMessage response)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"HTTP/{response.Version} {(int)response.StatusCode} {response.ReasonPhrase}");
        string key;
        IEnumerable<string> value;
        foreach (var header in response.Headers)
        {
            header.Deconstruct(out key, out value);
            var text = key;
            foreach (var item in value)
            {
                sb.AppendLine(text + ": " + item);
            }
        }

        foreach (var header2 in response.Content.Headers)
        {
            header2.Deconstruct(out key, out value);
            var text2 = key;
            foreach (var item2 in value)
            {
                sb.AppendLine(text2 + ": " + item2);
            }
        }

        sb.AppendLine();
        var value2 = await response.Content.ReadAsStringAsync();
        if (!string.IsNullOrWhiteSpace(value2))
        {
            sb.AppendLine(value2);
        }

        return sb.ToString();
    }
}