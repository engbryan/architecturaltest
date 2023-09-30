using Refit;
using System.Net;

namespace Bryan.Proof.Auth.Api.Tests.Utils;

public static class MockHttpResponseMessage
{
    public static ApiResponse<T> MockSuccessHttpResponseMessage<T>(this T response, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        using HttpResponseMessage httpResponseMessage = new(statusCode);
        using var apiResponse = new ApiResponse<T>(httpResponseMessage, response, new());
        return apiResponse;
    }

    public static async Task<ApiResponse<T>> MockFailHttpResponseMessage<T>(this T response, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        using HttpResponseMessage httpResponseMessage = new()
        {
            StatusCode = statusCode,
            Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(response))
        };
        using HttpRequestMessage httpRequestMessage = new();
        var apiException = await ApiException.Create(httpRequestMessage, HttpMethod.Get, httpResponseMessage, new());
        using var apiResponse = new ApiResponse<T>(httpResponseMessage, response, new(), apiException);
        return apiResponse;
    }
}