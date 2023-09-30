using Bryan.Dynamo.Abstractions.Interfaces;
using Bryan.Proof.Auth.Api.Test.TestBase;
using Bryan.Proof.Auth.Domain.Entities;
using Bryan.TokenAuth.Entities;
using Bryan.TokenAuth.Interfaces;
using Newtonsoft.Json;
using Serilog.Filters;
using System.Net;
using System.Reflection.PortableExecutable;
using Xunit;

namespace Bryan.Proof.Auth.Api.Test.Tests.User.GetInfo.ValidUserPass;

public class InvalidUserPassTest : ProofTestBase
{
    public static ITokenGenerator? TokenGenerator;

    public InvalidUserPassTest() : base(BlackBoxConfiguration) { }

    private static Action<IServiceCollection> BlackBoxConfiguration => blackboxServicesSetup =>
    {
        //var clockskew = new ClockSkew(new DateTime(2023, 1, 13, 19, 10, 0));

        TokenGenerator = Substitute.For<ITokenGenerator>();
        var generatorGetUserTokenData = JsonConvert.DeserializeObject<TokenResult>(File.ReadAllText("./BlackBoxTests/User/GetInfo/InvalidUserPass/Mocked/tokenGenerator.GetUserToken.json"));
        TokenGenerator.UserToken(Arg.Any<string>(), Arg.Any<string>()).Returns(generatorGetUserTokenData!);

        //blackboxServicesSetup.AddSingleton(clockskew);
        blackboxServicesSetup.AddSingleton(TokenGenerator);
    };

    [Fact]
    public void ShouldReturnUnauthorized401ForWrongEmailPassword()
    {
        //lock (ClockSkew)
        //{
        //Arrange
        var emailPasswordData = File.ReadAllText("./BlackBoxTests/User/GetInfo/InvalidUserPass/post.wrongEmailPassword.json");
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
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        tokenContent.Should().NotBeNullOrWhiteSpace(tokenContent);

        var token = JsonConvert.DeserializeObject<TokenResult>(tokenContent);
        token.Should().BeAssignableTo<TokenResult>();

        if (Application.IsRunningAsBlackBox)
        {
            Received.InOrder(() => TokenGenerator!.UserToken(requestObject.Email, requestObject.Password));
        }

        AssertTokenIsNotValid(token!);
        //}
    }

    private void AssertTokenIsNotValid(TokenResult token)
    {
        var authorizedMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"api/v1/test", UriKind.Relative),
            Headers = {
                { "Authorization", $"Bearer {token.Access_token}" }
            }
        };

        var assertResponse = HttpClient.SendAsync(authorizedMessage).Result;
        assertResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

    }

}