using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Bryan.TokenAuth.Implementations;

[ExcludeFromCodeCoverage]
public class GraphService : IGraphService
{
    private readonly ILogger<GraphService> _logger;
    private readonly SecurityConfig _securityConfig;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly HttpClient _client;

    public GraphService(
        ILogger<GraphService> logger,
        SecurityConfig securityConfig,
        ITokenGenerator tokenGenerator,
        IHttpClientFactory clientFactory
    )
    {
        _logger = logger;
        _securityConfig = securityConfig;
        _tokenGenerator = tokenGenerator;
        _client = clientFactory.CreateClient();
        _client.BaseAddress = new Uri(SecurityConstants.GRAPH_URL);
    }

    public async Task<Result<List<AppRoleAssignments>>> GetUserAppRoles(string email)
    {
        var url = $"users/{email}/appRoleAssignments";
        var (_, gp, gpError) = await GraphToken<AppRoleAssignmentsResult>(url);
        if (gp is null)
            return gpError;

        var ret = gp.Value
            .Where(v => v.ResourceDisplayName!.StartsWith(_securityConfig.Claim.ResourceDisplayNamePrefix))
            .ToList();

        return ret;
    }

    public async Task<Result<User>> GetUser(string email)
    {
        var url = $"users/{email}";
        var (_, gp, gpError) = await GraphToken<UserResult>(url);
        if (gp is null)
            return gpError;

        return new User(gp.GivenName + " " + gp.Surname, gp.OnPremisesSamAccountName, Culture.GetLanguage(gp.UsageLocation!));
    }

    public async Task<Result<List<AppRole>>> GetApplicationRoles()
    {
        var url = $"applications?$select=displayName, appId, appRoles&$filter=appId eq '{_securityConfig.AD.ClientId}'";
        var (_, gp, gpError) = await GraphToken<AppRolesApplicationsResult>(url);
        if (gp is null)
            return gpError;

        var ret = gp.Value
            .First(w => w.AppId == _securityConfig.AD.ClientId).AppRoles
            .Where(w => w.IsEnabled)
            .ToList();

        return ret;
    }

    private async Task<Result<T>> GraphToken<T>(string url)
    {
        var (_, graphToken, graphTokenError) = await _tokenGenerator.GenerateGraphToken();
        if (graphToken is null)
            return graphTokenError;

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.TryAddWithoutValidation("Authorization", "Bearer " + graphToken.Access_token);

        var response = await _client.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning($"{nameof(GraphService)}/{url} with statusCode {{statusCode}}", response.StatusCode);
            return new ResultException($"{nameof(GraphService)}/{url} with no data");
        }

        var rs = await response.Content.ReadAsStringAsync();
        var ret = JsonSerializer.Deserialize<T>(rs, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (ret is null)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogWarning($"{nameof(TokenGenerator)}/{url} with data {{@Data}}", rs);
            else
                _logger.LogWarning($"{nameof(TokenGenerator)}/oauth2/response) null");

            return new ResultException($"{nameof(TokenGenerator)}/oauth2/token with no data");
        }

        return ret;
    }
}