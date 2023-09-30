using Bryan.TokenAuth.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using static Bryan.TokenAuth.Config.SecurityConfig;

namespace Bryan.TokenAuth.IoC;

[ExcludeFromCodeCoverage]
public static class SecurityServiceCollection
{
    private const string BRYAN_TOKENAUTH = "Settings.Bryan-TokenAuth";
    private const string BRYAN_TOKENAUTH_PROVIDERS = $"Settings.{BRYAN_TOKENAUTH}:Providers";
    private const string BRYAN_TOKENAUTH_AZURE_AD = $"Settings.{BRYAN_TOKENAUTH}:AzureAD";

    public static IServiceCollection AddBryanTokenAuth<TMarketRepository>(this IServiceCollection services, IConfiguration configuration)
        where TMarketRepository : MarketTypeDAO
    {
        services.SecurityConfigLoad(configuration);

        services.AddScoped<IMarketRepository, TMarketRepository>();
        services.AddScoped<ITokenEvaluator, TokenEvaluator>();
        services.AddSingleton<ITokenGenerator, TokenGenerator>();
        services.AddSingleton<IGraphService, GraphService>();

        return services;
    }

    public static IServiceCollection AddBryanJwtAuthentication(this IServiceCollection services, IConfiguration configuration, string customAuthorizationHeader = "Authorization")
    {
        var securityConfig = services.SecurityConfigLoad(configuration);

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddMicrosoftIdentityWebApi
            (
                jwtOpt => SetupJwtBearerOptions(jwtOpt, customAuthorizationHeader),
                miOpt => configuration.Bind(BRYAN_TOKENAUTH_AZURE_AD.Replace("Settings.", ""), miOpt)
            )
            ;

        services.AddBryanAuthorization(securityConfig, new string[] { JwtBearerDefaults.AuthenticationScheme });

        return services;
    }

    public static IServiceCollection AddBryanJwtAuthenticationProviders(this IServiceCollection services, IConfiguration configuration, string customAuthorizationHeader = "Authorization")
    {
        var securityConfig = services.SecurityConfigLoad(configuration);
        
        List<Dictionary<string, string>> authsConfig = new();
        configuration.GetSection($"{BRYAN_TOKENAUTH_PROVIDERS.Replace("Settings.", "")}").Bind(authsConfig);
        if (authsConfig.Count == 0)
            throw new Exception($"{BRYAN_TOKENAUTH_PROVIDERS} is empty");

        var builder = services.AddAuthentication(options =>
        {
            var schema = authsConfig[0]["SchemaName"] ?? JwtBearerDefaults.AuthenticationScheme;
            options.DefaultAuthenticateScheme = schema;
            options.DefaultChallengeScheme = schema;
        });

        //var signingKeys = new List<SecurityKey>();
        //var issuers = new HashSet<string>();
        foreach (var auth in authsConfig)
        {
            if (string.IsNullOrEmpty(auth["Type"])) throw new Exception($"{BRYAN_TOKENAUTH_PROVIDERS}.Type is required");
            if (string.IsNullOrEmpty(auth["SchemaName"])) throw new Exception($"{BRYAN_TOKENAUTH_PROVIDERS}.SchemaName is required");

            if (auth["Type"] == "AzureAD")
            {
                if (string.IsNullOrEmpty(auth[nameof(ActiveDirectoryConfig.ClientId)])) throw new Exception($"{BRYAN_TOKENAUTH_PROVIDERS}.{nameof(ActiveDirectoryConfig.ClientId)} is required");
                if (string.IsNullOrEmpty(auth[nameof(ActiveDirectoryConfig.TenantId)])) throw new Exception($"{BRYAN_TOKENAUTH_PROVIDERS}.{nameof(ActiveDirectoryConfig.TenantId)} is required");
                if (string.IsNullOrEmpty(auth[nameof(ActiveDirectoryConfig.ApplicationIdUri)])) throw new Exception($"{BRYAN_TOKENAUTH_PROVIDERS}.{nameof(ActiveDirectoryConfig.ApplicationIdUri)} is required");
                if (string.IsNullOrEmpty(auth[nameof(ActiveDirectoryConfig.ClientId)])) throw new Exception($"{BRYAN_TOKENAUTH_PROVIDERS}.{nameof(ActiveDirectoryConfig.ClientId)} is required");
                if (string.IsNullOrEmpty(auth[nameof(ActiveDirectoryConfig.ClientSecret)])) throw new Exception($"{BRYAN_TOKENAUTH_PROVIDERS}.{nameof(ActiveDirectoryConfig.ClientSecret)} is required");
                if (string.IsNullOrEmpty(auth[nameof(ActiveDirectoryConfig.UserAgent)])) throw new Exception($"{BRYAN_TOKENAUTH_PROVIDERS}.{nameof(ActiveDirectoryConfig.UserAgent)} is required");

                //var configManager = new ConfigurationManager<OpenIdConnectConfiguration>(auth[nameof(ActiveDirectoryConfig.Instance)], new OpenIdConnectConfigurationRetriever());
                //var openidconfig = configManager.GetConfigurationAsync().Result;
                //issuers.Add(openidconfig.Issuer);
                //signingKeys.AddRange(openidconfig.SigningKeys);

                builder.AddMicrosoftIdentityWebApi(
                    jwtOpt => SetupJwtBearerOptions(jwtOpt, customAuthorizationHeader),
                    miOpt =>
                    {
                        miOpt.Instance = auth[nameof(ActiveDirectoryConfig.Instance)];
                        miOpt.TenantId = auth[nameof(ActiveDirectoryConfig.TenantId)];
                        miOpt.Domain = auth[nameof(ActiveDirectoryConfig.ApplicationIdUri)];
                        miOpt.ClientId = auth[nameof(ActiveDirectoryConfig.ClientId)];
                        miOpt.ClientSecret = auth[nameof(ActiveDirectoryConfig.ClientSecret)];
                    },
                    auth["SchemaName"]
                );
            }
            else
            {
                throw new Exception($"{BRYAN_TOKENAUTH_PROVIDERS}.Type = {auth["Type"]} not expected");
            }
        }

        services.AddBryanAuthorization(securityConfig, authsConfig.Select(p => p["SchemaName"]).ToArray());

        //tokenValidation.IssuerSigningKeys = signingKeys;
        //tokenValidation.ValidIssuers = issuers.ToArray();
        return services;
    }

    public static IServiceCollection AddBryanAuthorization(this IServiceCollection services, SecurityConfig securityConfig, string[] authenticationSchemes)
    {
        services.AddAuthorization(options =>
        {
            AuthRequirement authRequirement = new();
            foreach (var policyItem in securityConfig.Claim.Policies)
            {
                //policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                options.AddPolicy(policyItem.Name, policy =>
                {
                    policy.RequireClaim(ClaimTypes.Role, policyItem.Roles);
                    policy.Requirements.Add(authRequirement);
                });
            }

            var authorizarionPolicyBuilder = new AuthorizationPolicyBuilder(authenticationSchemes);
            authorizarionPolicyBuilder.RequireAuthenticatedUser();
            options.DefaultPolicy = authorizarionPolicyBuilder.Build();
        });

        services.AddSingleton<IAuthorizationHandler, AuthorizationHandlerExtension>();
        return services;
    }

    private static SecurityConfig SecurityConfigLoad(this IServiceCollection services, IConfiguration configuration)
    {
        var securityConfig = (SecurityConfig)services.FirstOrDefault(p => p.ServiceType == typeof(SecurityConfig))?.ImplementationInstance;
        if (securityConfig is not null)
            return securityConfig;

        securityConfig = new();
        configuration.GetSection(BRYAN_TOKENAUTH.Replace("Settings.", "")).Bind(securityConfig);
        configuration.GetSection(BRYAN_TOKENAUTH_AZURE_AD.Replace("Settings.", "")).Bind(securityConfig.AD);

        var roleCredentialsNotFound = string.Join(",",
            securityConfig?.Claim?.Roles?.SelectMany(p => p.Credentials).Distinct()
            .Where(credential => !securityConfig.Credentials?.Any(c => c.Name == credential) ?? true)
            .ToArray() ?? new string[] { }
        );
        if (!string.IsNullOrEmpty(roleCredentialsNotFound))
            throw new ArgumentException($"{BRYAN_TOKENAUTH_PROVIDERS}.{nameof(securityConfig.Claim)}.{nameof(securityConfig.Claim.Roles)}.{nameof(securityConfig.Credentials)} not found in {BRYAN_TOKENAUTH_PROVIDERS}.{nameof(securityConfig.Credentials)}.Name: {roleCredentialsNotFound}");

        //var policeRolesNotFound = string.Join(",",
        //    securityConfig?.Claim?.Policies?.SelectMany(p => p.Roles).Distinct()
        //    .Where(role => !securityConfig.Claim.Roles?.Any(c => c.Name == role) ?? true)
        //    .ToArray()
        //);
        //if (!string.IsNullOrEmpty(policeRolesNotFound))
        //    throw new ArgumentException($"{Bryan_TOKENAUTH_SESSION_NAME}.{nameof(securityConfig.Claim)}.{nameof(securityConfig.Claim.Policies)}.{nameof(securityConfig.Claim.Roles)} not found in {Bryan_TOKENAUTH_SESSION_NAME}.{nameof(securityConfig.Claim)}.{nameof(securityConfig.Claim.Roles)}.Name: {policeRolesNotFound}");

        services.AddSingleton(securityConfig);

        return securityConfig;
    }

    private static void SetupJwtBearerOptions(Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerOptions options, string customAuthorizationHeader)
    {
        //options.TokenValidationParameters.RoleClaimType = "roles";
        //options.TokenValidationParameters.NameClaimType = "name";
        options.TokenValidationParameters.ValidateIssuer = false;
        options.TokenValidationParameters.ValidateIssuerSigningKey = false;
        options.TokenValidationParameters.RequireExpirationTime = false;
        options.TokenValidationParameters.ValidateLifetime = false;

        options.BackchannelHttpHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (httpRequestMessage, x509Certificate2, x509Chain, sslPolicyErrors) => true
        };

        options.Events = new()
        {
            OnMessageReceived = context => ReadFromOtherHeader(context, customAuthorizationHeader)
        };
    }

    private static Task ReadFromOtherHeader(MessageReceivedContext context, string headerName)
    {
        string text = null;
        if (
            context.Request.Headers.TryGetValue(headerName, out var value) ||
            context.Request.Headers.TryGetValue("Authorization", out value)
        )
        {
            text = value;
        }

        if (!string.IsNullOrEmpty(text) && text.StartsWith("Bearer "))
        {
            context.Token = text["Bearer ".Length..];
        }

        return Task.CompletedTask;
    }

    //private static void SetupMicrosoftIdentityOptions(MicrosoftIdentityOptions options, string customAuthorizationHeader)
    //{
    //    //options.TokenValidationParameters.RoleClaimType = "roles";
    //    //options.TokenValidationParameters.NameClaimType = "name";
    //    options.TokenValidationParameters.ValidateLifetime = true;
    //    options.TokenValidationParameters.ValidateIssuer = true;
    //    options.TokenValidationParameters.ValidateIssuerSigningKey = true;

    //    options.BackchannelHttpHandler = new HttpClientHandler
    //    {
    //        ServerCertificateCustomValidationCallback = (httpRequestMessage, x509Certificate2, x509Chain, sslPolicyErrors) => true
    //    };

    //    options.Events = new()
    //    {
    //        OnMessageReceived = context => ReadFromOtherHeader(context, customAuthorizationHeader)
    //    };
    //}

    //private static Task ReadFromOtherHeader(Microsoft.AspNetCore.Authentication.OpenIdConnect.MessageReceivedContext context, string headerName)
    //{
    //    string text = null;
    //    if (
    //        context.Request.Headers.TryGetValue(headerName, out var value) ||
    //        context.Request.Headers.TryGetValue("Authorization", out value)
    //    )
    //    {
    //        text = value;
    //    }

    //    if (!string.IsNullOrEmpty(text) && text.StartsWith("Bearer"))
    //    {
    //        context.Token = text["Bearer ".Length..];
    //    }

    //    return Task.CompletedTask;
    //}

    //public static async Task OnTokenValidated(TokenValidatedContext ctx, SecurityConfig securityConfig)
    //{
    //    var generator = ctx.HttpContext.RequestServices.GetRequiredService<ITokenGenerator>();

    //    var hasRefresh = ctx.Request.Headers.TryGetValue("Refresh_Token", out var refreshToken);

    //    TokenResult newToken;
    //    if (!hasRefresh)
    //    {
    //        var evaluator = ctx.HttpContext.RequestServices.GetRequiredService<TokenEvaluator>();
    //        var token = ctx.SecurityToken as JwtSecurityToken;
    //        var (success, _, isSystemTokenError) = await evaluator.IsSystemToken(token!);
    //        if (!success)
    //        {
    //            var logger = ctx.HttpContext.RequestServices.GetRequiredService<ILogger<SecurityServiceCollection>>();
    //            logger.LogError($"{nameof(OnTokenValidated)}.{nameof(evaluator.IsSystemToken)}.error {{error}}", isSystemTokenError);
    //            return;
    //        }

    //        var (_, getSystemToken, getSystemTokenError) = await generator.SystemToken();
    //        if (!success)
    //        {
    //            var logger = ctx.HttpContext.RequestServices.GetRequiredService<ILogger<SecurityServiceCollection>>();
    //            logger.LogError($"{nameof(OnTokenValidated)}.{nameof(generator.SystemToken)}.error {{error}}", getSystemTokenError);
    //            return;
    //        }

    //        newToken = getSystemToken;
    //    }
    //    else
    //    {
    //        var (success, refreshTokenRs, refreshTokenError) = await generator.RefreshToken(refreshToken);
    //        if (!success)
    //        {
    //            var logger = ctx.HttpContext.RequestServices.GetRequiredService<ILogger<SecurityServiceCollection>>();
    //            logger.LogError($"{nameof(OnTokenValidated)}.{nameof(generator.RefreshToken)}.error {{error}}", refreshTokenError);
    //            return;
    //        }

    //        newToken = refreshTokenRs;
    //    }

    //    ctx.Properties.AllowRefresh = true;

    //    ctx.Response.Headers.Add(securityConfig.Token.HttpHeaderName, new StringValues(newToken.Access_token));
    //    ctx.Response.Headers.Add("Refresh_Token", new StringValues(newToken.Refresh_token));

    //    return;
    //}
}