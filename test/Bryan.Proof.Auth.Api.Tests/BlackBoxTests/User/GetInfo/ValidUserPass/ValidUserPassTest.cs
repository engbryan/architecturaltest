using Bryan.Dynamo.Abstractions.Interfaces;
using Bryan.Proof.Auth.Api.Test.TestBase;
using Bryan.TokenAuth.Entities;
using Bryan.TokenAuth.Interfaces;
using Newtonsoft.Json;
using Serilog.Filters;
using System.Net;

namespace Bryan.Proof.Auth.Api.Test.Tests.User.GetInfo.ValidUserPass;

public class ValidUserPassTest : ProofTestBase
{
    public static ITokenGenerator? TokenGenerator;

    public ValidUserPassTest() : base(BlackBoxConfiguration) { }

    private static Action<IServiceCollection> BlackBoxConfiguration => blackboxServicesSetup =>
    {
        //var clockskew = new ClockSkew(new DateTime(2023, 1, 13, 19, 10, 0));

        TokenGenerator = Substitute.For<ITokenGenerator>();
        var generatorGetUserTokenData = JsonConvert.DeserializeObject<TokenResult>(File.ReadAllText("./BlackBoxTests/User/GetInfo/ValidUserPass/Mocked/tokenGenerator.GetUserToken.json"));
        TokenGenerator.UserToken(Arg.Any<string>(), Arg.Any<string>()).Returns(generatorGetUserTokenData!);

        //blackboxServicesSetup.AddSingleton(clockskew);
        blackboxServicesSetup.AddSingleton(TokenGenerator);
    };

    [Fact]
    public void ShouldReturnOk200AndTokenForValidEmailPassword()
    {
        //lock (ClockSkew)
        //{
            //Arrange
            var emailPasswordData = File.ReadAllText("./BlackBoxTests/User/GetInfo/ValidUserPass/post.correctEmailPassword.json");
            var requestObject = JsonConvert.DeserializeObject<InfoByEmailRq>(emailPasswordData)!;

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"api/v1/info-by-email", UriKind.Relative),
                Content = JsonContent.Create(requestObject)
            };

            //Act 
            var response = HttpClient.SendAsync(httpRequestMessage).Result;
            var tokenContent = response.Content.ReadAsStringAsync().Result;

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            tokenContent.Should().NotBeNullOrWhiteSpace(tokenContent);

            var token = JsonConvert.DeserializeObject<InfoRs>(tokenContent);
            token.Should().BeAssignableTo<InfoRs>();

            if (Application.IsRunningAsBlackBox)
            {
                Received.InOrder(() =>
                {
                    TokenGenerator!.UserToken(requestObject.Email, requestObject.Password);
                    MarketRepository!.GetAll();
                    GraphService!.GetUser(requestObject.Email);
                    //TraceRepository!.Save(Arg.Any<TraceRecord>());
                });
            }

            AssertTokenValidity(requestObject.Email, token!);
        //}
    }

    private void AssertTokenValidity(string email, InfoRs token)
    {
        var authorizedMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"api/v1/test", UriKind.Relative),
            Headers = {
                { "Authorization", $"Bearer {token.Token}" }
            }
        };

        var assertResponse = HttpClient.SendAsync(authorizedMessage).Result;
        assertResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        if (Application.IsRunningAsBlackBox)
        {
            Received.InOrder(() =>
            {
                MarketRepository!.GetAll();
                GraphService!.GetUser(email);
                //MarketRepository!.GetAll();
                //GraphService!.GetApplicationRoles();
                //tokenGenerator!.GetSystemToken(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
            });
        }
    }

}