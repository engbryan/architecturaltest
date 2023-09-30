using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace Bryan.TokenAuth.Implementations;

[ExcludeFromCodeCoverage]
public class AuthorizationHandlerExtension : AuthorizationHandler<AuthRequirement>
{
    private readonly ILogger<AuthorizationHandlerExtension> _logger;
    private readonly SecurityConfig _securityConfig;
    //private readonly IGraphService _graphService;
    public AuthorizationHandlerExtension(ILogger<AuthorizationHandlerExtension> logger
        , SecurityConfig securityConfig
    //,IGraphService graphService
    )
    {
        _logger = logger;
        _securityConfig = securityConfig;
        //_graphService = graphService;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthRequirement requirement)
    {
        if (!context?.User?.Identity?.IsAuthenticated ?? false)
        {
            context.Fail();
            return Task.CompletedTask;
        }

        try
        {
            var userRoles = context.User.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();

            if (context.Resource is HttpContext httpContext)
            {
                var endpoint = httpContext.GetEndpoint();
                var endPointPolicies = endpoint.Metadata.GetOrderedMetadata<AuthorizeAttribute>();

                if (endPointPolicies.Count == 1 && string.IsNullOrEmpty(endPointPolicies[0].Policy))
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }

                if (_securityConfig?.Claim?.Policies?.Any(p =>
                    endPointPolicies.Any(a => a.Policy == p.Name) &&
                    p.Roles.Any(r => userRoles.Any(ur => ur.Value == r))
                ) ?? false)
                {
                    foreach(var req in context.PendingRequirements)
                        context.Succeed(req);
                    return Task.CompletedTask;
                }
                else
                {
                    context.Fail();
                    return Task.CompletedTask;
                }
            }

            context.Succeed(requirement);
            return Task.CompletedTask;

            //var (_, appRoles, appRolesError) = await _graphService.GetApplicationRoles();
            //if (appRoles == null)
            //{
            //    _logger.LogInformation("AuthorizationHandler: {message}", appRolesError.Message);
            //    context.Fail();
            //}

            //if (roles.Any(r => appRoles.Any(or => or.DisplayName == r.Value)))
            //{
            //    context.Succeed(requirement);
            //}
            //else
            //    context.Fail();
        }
        catch (Exception ex)
        {
            _logger.LogError("AuthorizationHandler: {error}", ex);
            context.Fail();
        }
        return Task.CompletedTask;
    }
}