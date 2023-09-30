using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Web;

namespace Bryan.TokenAuth.Implementations;

[ExcludeFromCodeCoverage]
public class TokenGenerator : ITokenGenerator
{
    private readonly ILogger<TokenGenerator> _logger;
    private readonly SecurityConfig.ActiveDirectoryConfig _adConfig;

    public TokenGenerator(ILogger<TokenGenerator> logger, SecurityConfig securityConfig)
    {
        _logger = logger;
        _adConfig = securityConfig.AD;
    }

    public async Task<Result<TokenResult>> GenerateGraphToken() => await SystemToken(SecurityConstants.GRAPH_SCOPE);

    public async Task<Result<TokenResult>> SystemToken() => await SystemToken(_adConfig.ApplicationIdUri);

    public async Task<Result<TokenResult>> SystemToken(string applicationIdUri)
    {
        var url =
            $"grant_type=client_credentials" +
            $"&client_id={_adConfig.ClientId}" +
            $"&client_secret={_adConfig.ClientSecret}" +
            $"&scope={applicationIdUri} offline_access";
        return await AD(new StringContent(url));
    }

    public async Task<Result<TokenResult>> UserToken(string username, string password)
    {
        var url =
            "grant_type=password" +
            $"&client_id={_adConfig.ClientId}" +
            $"&client_secret={_adConfig.ClientSecret}" +
            $"&username={username}" +
            $"&password={HttpUtility.UrlEncode(password, Encoding.ASCII)}" +
            $"&scope={_adConfig.ApplicationIdUri} offline_access";
        return await AD(new StringContent(url, Encoding.ASCII));
    }

    public async Task<Result<TokenResult>> RefreshToken(string refreshToken)
    {
        var url =
           "grant_type=refresh_token" +
           $"&client_id={_adConfig.ClientId}" +
           $"&client_secret={_adConfig.ClientSecret}" +
           $"&refresh_token={refreshToken}" +
           $"&scope={_adConfig.ApplicationIdUri}";
        return await AD(new StringContent(url));
    }

    private async Task<Result<TokenResult>> AD(StringContent content)
    {
        var client = new HttpClient { BaseAddress = new Uri(_adConfig.Instance) };
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_adConfig.TenantId}/oauth2/v2.0/token") { Content = content };
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
        request.Headers.TryAddWithoutValidation("user-agent", _adConfig.UserAgent);

        var response = await client.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning($"{nameof(TokenGenerator)}/oauth2/token with statusCode {{statusCode}}", response.StatusCode);
            return new ResultException((int)response.StatusCode, $"{nameof(TokenGenerator)}/oauth2/token with statusCode {response.StatusCode}");
        }

        var rs = await response.Content.ReadAsStringAsync();
        var ret = JsonSerializer.Deserialize<TokenResult>(rs, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
        if (ret is null)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogWarning($"{nameof(TokenGenerator)}/oauth2/token with response null with data {{@Data}}", rs);
            else
                _logger.LogWarning($"{nameof(TokenGenerator)}/oauth2/token with response null");

            return new ResultException($"{nameof(TokenGenerator)}/oauth2/token with no data");
        }

        return ret;
    }
}