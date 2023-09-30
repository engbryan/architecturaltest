using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Runtime.CredentialManagement;
using Bryan.Dynamo.Abstractions.Interfaces;
using Bryan.Dynamo.Abstractions.Repositories;
using Bryan.HttpClients.Extensions;
using Bryan.MediatR.Extensions;
using Bryan.Proof.Auth.Domain.Repositories;
using Bryan.Proof.Auth.IoC.Config;
using Bryan.Swagger;
using Bryan.TokenAuth.IoC;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using NetCore.AutoRegisterDi;
using Refit;
using System.Diagnostics.CodeAnalysis;

namespace Bryan.Proof.Auth.Api.Configuration;

[ExcludeFromCodeCoverage]
public static class AppServiceCollectionExtensions
{
    public const string APP_NAME = "Bryan.Proof.Auth.Api";
    public const string PRODUCTION = "Production";
    public const string DEVELOPMENT = "Development";
    public const string ASPNETCORE_ENVIRONMENT = "ASPNETCORE_ENVIRONMENT";

    public static IConfigurationRoot BuildConfiguration(string environmentDefault = PRODUCTION)
    {
        var env = Environment.GetEnvironmentVariable(ASPNETCORE_ENVIRONMENT) ?? environmentDefault;

        return new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{env}.json")
            .Build();
    }

    public static bool IsProd(this IConfiguration configuration) => configuration[ASPNETCORE_ENVIRONMENT] == PRODUCTION;

    public static void ConfigureAppDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        AppConfig appConfig = new();
        configuration.Bind(appConfig);
        services.AddSingleton(appConfig);

        services.AddHttpContextAccessor();

        services.ConfigurePipelineBehaviors();

        services.AddAWSServices(configuration);

        services.AddBryanTokenAuth<MarketTypeRepository>(configuration);

        services.ConfigureRefit(configuration);

        services.ConfigureServices();
    }

    private static void ConfigurePipelineBehaviors(this IServiceCollection services)
    {
        services.AddMediatR(new[] { typeof(DomainEntryPoint).Assembly });
        services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(ExceptionPipelineBehavior<,>));
        services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(ValidatorPipelineBehavior<,>));
        services.ConfigureFluentValidation(new[] { typeof(DomainEntryPoint).Assembly });
    }

    private static void AddAWSServices(this IServiceCollection services, IConfiguration configuration)
    {
#if DEBUG
        var chain = new CredentialProfileStoreChain();
        chain.TryGetAWSCredentials("Bryan-Proof-dev", out var credential);
        services.AddSingleton<IAmazonDynamoDB>(s => new AmazonDynamoDBClient(credential, RegionEndpoint.USEast1));
#else
        var region = configuration.IsProd() ? RegionEndpoint.SAEast1 : RegionEndpoint.USEast1;
        services.AddSingleton<IAmazonDynamoDB>(s => new AmazonDynamoDBClient(region));
#endif
    }

    private static void ConfigureRefit(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHeaderPropagation(options =>
        {
            options.Headers.Add("Authorization", ctx => ctx.HttpContext.Request.Headers.ContainsKey(SecurityRequirementsOperationFilter.OAuthScheme.Name)
                    ? ctx.HttpContext.Request.Headers[SecurityRequirementsOperationFilter.OAuthScheme.Name]
                    : ctx.HeaderValue);
            options.Headers.Add("X-Correlation-Id");
        });

        //services.AddSingleton<IRawHttpRequestResponseExporter, RawHttpRequestResponseExporter>();

        ConfigureXxxRefitService(services, configuration);
    }

    private static void ConfigureXxxRefitService(IServiceCollection services, IConfiguration configuration)
        => ConfigureRefitService<IServiceCollection>(services, configuration["Servicos:Xxx:Address"])
            .AddHeaderPropagation();

    private static IHttpClientBuilder ConfigureRefitService<T>(IServiceCollection services, string uri)
        where T : class => services.ConfigureRefitService<T>(uri, typeof(Program).Assembly.GetName().Name!);

    private static void ConfigureServices(this IServiceCollection services)
    {
        services.RegisterAssemblyPublicNonGenericClasses(typeof(MarketTypeRepository).Assembly)
            .Where(x => x.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IDynamoRepository<,>)))
            .AsPublicImplementedInterfaces(ServiceLifetime.Transient);

        services.AddScoped<IRepositoryFactory, RepositoryFactory>();
    }
}