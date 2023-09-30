using Bryan.Proof.Auth.Api.Tests.TestBase;
using Bryan.Proof.Auth.Domain.Entities;
using Bryan.Proof.Auth.Domain.Repositories;
using Bryan.TokenAuth.Entities;
using Bryan.TokenAuth.Interfaces;
using Bryan.TokenAuth.Repositories;
using Newtonsoft.Json;

namespace Bryan.Proof.Auth.Api.Test.TestBase;

public class ProofTestBase
{
    internal TestApplication Application { get; private set; }
    public HttpClient HttpClient { get; private set; }
    public IGraphService? GraphService { get; private set; }
    public IMarketRepository? MarketRepository { get; private set; }
    public ITraceRepository? TraceRepository { get; private set; }

    //public ClockSkew ClockSkew { get; set; }

    public ProofTestBase(Action<IServiceCollection>? additionalSetupServices = default)
    {
        Application = new TestApplication(setupServices =>
        {
            ConfigureBlackBoxServices(setupServices);
            additionalSetupServices?.Invoke(setupServices);
        });
        HttpClient = Application.CreateClient();

        //ClockSkew = Application.Services.GetService<ClockSkew>() ?? new ClockSkew();
    }

    protected virtual void ConfigureBlackBoxServices(IServiceCollection blackboxServicesSetup)
    {
        MarketRepository = Substitute.For<IMarketRepository>();
        var staticMarketData = JsonConvert.DeserializeObject<List<MarketType>>(File.ReadAllText("./TestBase/Mocked/marketRepository.GetAll.json"));
        MarketRepository.GetAll().Returns(staticMarketData!);

        GraphService = Substitute.For<IGraphService>();
        var graphGetUserData = JsonConvert.DeserializeObject<User>(File.ReadAllText("./TestBase/Mocked/graphService.GetUser.json"));
        GraphService.GetUser(Arg.Any<string>()).Returns(graphGetUserData!);
        var graphGetApplicationRolesData = JsonConvert.DeserializeObject<List<AppRole>>(File.ReadAllText("./TestBase/Mocked/graphService.GetApplicationRoles.json"));
        GraphService.GetApplicationRoles().Returns(graphGetApplicationRolesData!);

        TraceRepository = Substitute.For<ITraceRepository>();
        TraceRepository.When(x => x.Save(Arg.Any<TraceRecord>()))
            .Do(callInfo => Console.WriteLine($"Attempted to Save a trace with object: '{callInfo.Args().First()}'"));

        blackboxServicesSetup.AddSingleton(MarketRepository);
        blackboxServicesSetup.AddSingleton(GraphService);
        blackboxServicesSetup.AddSingleton(TraceRepository);
    }

}