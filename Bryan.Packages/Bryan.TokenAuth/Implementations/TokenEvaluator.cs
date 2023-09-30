using Bryan.TokenAuth.Config;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace Bryan.TokenAuth.Implementations;

[ExcludeFromCodeCoverage]
public class TokenEvaluator : ITokenEvaluator
{
    private readonly ILogger<TokenEvaluator> _logger;
    private readonly SecurityConfig _securityConfig;
    private readonly IMarketRepository _marketRepository;
    private readonly IGraphService _graphService;

    public TokenEvaluator(
        ILogger<TokenEvaluator> logger,
        SecurityConfig securityConfig,
        IMarketRepository marketRepository,
        IGraphService graphService
    )
    {
        _logger = logger;
        _securityConfig = securityConfig;
        _marketRepository = marketRepository;
        _graphService = graphService;
    }

    public async Task<Result<SecurityEvaluationResult>> EvaluateRoles(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
        
        var tokenRoleList = securityToken!.Claims.Where(claim => claim.Type == _securityConfig.Claim.RoleClaimType);

        var (_, marketTypes, marketListrror) = await _marketRepository.GetAll();
        if (marketTypes is null)
            return marketListrror;

        return ParseRoles(true, tokenRoleList.Select(p => p.Value).ToArray(), marketTypes);
    }

    public async Task<Result<SecurityEvaluationResult>> EvaluateUser(string email, Guid marketCode)
    {
        var (_, roles, rolesError) = await _graphService.GetUserAppRoles(email);
        if (roles is null)
            return rolesError;

        var (_, marketType, marketsError) = await _marketRepository.GetByGuid(marketCode.ToString());
        if (marketType is null)
            return marketsError;

        return ParseRoles(false, roles.Select(p => p.AppRoleId).ToArray(), new List<MarketType>() { marketType });
    }
    private Result<SecurityEvaluationResult> ParseRoles(bool rolyesByName, string[] roles, List<MarketType> marketTypes)
    {
        var apiList = new List<ApiConfig>();
        var userRoleList = new List<string>();
        var found = false;

        foreach (var roleConfig in _securityConfig.Claim?.Roles)
        {
            found = rolyesByName ? roles.Any(r => r == roleConfig.Name) : roles.Any(r => r == roleConfig.Id);
            if (found)
            {
                userRoleList.Add(roleConfig.Name);
                foreach (var credentialName in roleConfig.Credentials)
                {
                    var credential = _securityConfig.Credentials.First(r => r.Name == credentialName);
                    apiList.Add(new ApiConfig(credential.Key, credential.Value));
                }
                break;
            }
        }

        if (!found)
        {
            foreach (var role in roles)
            {
                var market = rolyesByName ?
                    marketTypes.FirstOrDefault(m => m.RoleList.Any(r => r.Name == role)) :
                    marketTypes.FirstOrDefault(m => m.RoleList.Any(r => r.Id == role));

                if (market is not null)
                {
                    found = true;
                    apiList = market.ApiList.Select(p => new ApiConfig(p.Key, Convert.ToInt32(p.Value))).ToList();
                }

                userRoleList.Add(role);
            }
        }

        var ret = new SecurityEvaluationResult() { ApiList = apiList, Passed = found, Roles = userRoleList };
        if (_logger.IsEnabled(LogLevel.Debug))
            _logger.LogWarning($"{nameof(TokenEvaluator)}.{nameof(EvaluateRoles)} with data {{@Data}}", ret);

        return ret;
    }

}