using Bryan.Proof.Auth.Domain;
using Bryan.Proof.Auth.Api.Configuration;
using Microsoft.AspNetCore.Http;

namespace Bryan.Proof.Auth.Api.Tests;

public class DependencyInjectionConfigurationTests 
{
    [Fact]
    public void AllHandlersMustBeInitialized()
    {
        var configuration = AppServiceCollectionExtensions.BuildConfiguration(AppServiceCollectionExtensions.DEVELOPMENT);
        var serviceCollection = new ServiceCollection();
        
        serviceCollection.AddSingleton(Substitute.For<IHttpContextAccessor>());

        serviceCollection.ConfigureAppDependencies(configuration);

        var serviceProvider = serviceCollection.BuildServiceProvider(true);
        using var scope = serviceProvider.CreateScope();

        var requestTHandlerType = typeof(IRequestHandler<,>);

        var handlers = typeof(DomainEntryPoint)
            .Assembly
            .ExportedTypes
            .SelectMany(p => p.GetInterfaces(), (@class, @interface) => (@class, @interface))
            .Where(p => p.@interface.IsGenericType)
            .Where(p => p.@interface.GetGenericTypeDefinition() == requestTHandlerType);

        foreach (var (handlerType, @interface) in handlers)
            scope.ServiceProvider.GetRequiredService(@interface);
    }
}
