using System.Text;

namespace Bryan.Proof.Auth.Api.Tests.Utils;

internal static class HttpClientExtensions
{
    public static Task<HttpResponseMessage> PostAsync(this HttpClient httpClient, string requestUri, string content, string media = "application/json") =>
        httpClient.PostAsync(requestUri, new StringContent(content, Encoding.UTF8, media));
}