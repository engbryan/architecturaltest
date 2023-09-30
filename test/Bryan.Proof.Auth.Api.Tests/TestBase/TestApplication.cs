using Bryan.Proof.Auth.Api.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Bryan.Proof.Auth.Api.Tests.TestBase;

internal class TestApplication : WebApplicationFactory<Program>
{
    private readonly Action<IServiceCollection>? _setupServices;
    private readonly IConfigurationRoot _configuration;

    public bool IsRunningAsBlackBox { get; private set; }

    public TestApplication(Action<IServiceCollection>? setupServices = default)
    {
        _configuration = AppServiceCollectionExtensions.BuildConfiguration(AppServiceCollectionExtensions.DEVELOPMENT);

        IsRunningAsBlackBox = _configuration["TestMode"]?.ToLower() == "blackbox";
        _setupServices = setupServices;
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(AppServiceCollectionExtensions.ASPNETCORE_ENVIRONMENT)))
            builder.UseEnvironment(AppServiceCollectionExtensions.DEVELOPMENT);

        builder.ConfigureAppConfiguration((x, _) => x.Configuration["urls"] = "*");

        if (_setupServices is not null)
        {
            builder.ConfigureServices(services => _setupServices?.Invoke(services));
        }
        else
        {
            var mediator = Substitute.For<IMediator>();
            var asp = Substitute.For<IAuthenticationSchemeProvider>();
            asp.GetSchemeAsync(Arg.Any<string>()).Returns(new AuthenticationScheme(JwtBearerDefaults.AuthenticationScheme, JwtBearerDefaults.AuthenticationScheme, typeof(MockAuthenticationHandler)));

            builder.ConfigureServices(services =>
            {
                services.AddTransient(_ => mediator);
                //services.AddTransient(_ => new xDbContext(new DbCOntextOptionBuilder<xDbContext>().UseInMemoryDataBase("xDbContext").Options));
                services.AddTransient(_ => asp);
            });
        }

        return base.CreateHost(builder);
    }
}
