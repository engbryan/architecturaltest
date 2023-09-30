using Bryan.Proof.Auth.Api.Client.Exceptions;
using Bryan.Proof.Auth.Api.UnitTests;
using Bryan.TokenAuth.Entities;
using Bryan.TokenAuth.Implementations;
using Bryan.TokenAuth.Interfaces;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office.Word;
using NSubstitute;

namespace Bryan.Proof.Auth.Api.Tests.UnitTests.Handlers;

public class InfoByEmailHandlerTests
{
    private readonly IMediator _mediator = Substitute.For<IMediator>();
    private readonly ILogger<InfoByEmailHandler> _logger = Substitute.For<ILogger<InfoByEmailHandler>>();
    private readonly ITokenEvaluator _tokenEvaluator = Substitute.For<ITokenEvaluator>();
    private readonly ITokenGenerator _tokenGenerator = Substitute.For<ITokenGenerator>();
    private readonly IGraphService _graphService = Substitute.For<IGraphService>();
    private readonly InfoByEmailHandler _sut;

    public InfoByEmailHandlerTests()
        => _sut = new InfoByEmailHandler(_mediator, _logger, _tokenEvaluator, _tokenGenerator, _graphService);

    [Fact]
    public async Task UserToken_EvaluateRoles_GetUser_OK()
    {
        //Arrange
        InfoByEmailCmd rq = new("email", "password");
        TokenResult tokenResult = new() { Access_token = "Access_token", Refresh_token = "Refresh_token" };
        SecurityEvaluationResult securityEvaluationResult = new()
        {
            Passed = true,
            Roles = new List<string>() { "role1" },
            ApiList = new List<ApiConfig>() { new("key", 0) }
        };
        User user = new("Name", "UserName", "Language");

        _tokenGenerator.UserToken(rq.Email, rq.Password).Returns(tokenResult);
        _tokenEvaluator.EvaluateRoles(tokenResult.Access_token).Returns(securityEvaluationResult);
        _graphService.GetUser(rq.Email).Returns(user);

        //Act
        var (sucesso, _rs, _) = await _sut.Handle(rq, CancellationToken.None);
        var rs = _rs!;

        //Assert
        sucesso.Should().BeTrue();

        rs.Should().NotBeNull();
        rs.Token.Should().Be(tokenResult.Access_token);
        rs.Refresh.Should().Be(tokenResult.Refresh_token);
        rs.Expiry.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(15), TimeSpan.FromSeconds(30));
        rs.ApiList.Should().HaveCountGreaterThan(0);
        rs.User.Should().NotBeNull().And.BeEquivalentTo(user);
        rs.User.Roles.Should().NotBeNullOrEmpty().And.BeEquivalentTo(securityEvaluationResult.Roles);
        rs.ApiList.Count.Should().Be(securityEvaluationResult.ApiList.Count);

        await _tokenGenerator.Received().UserToken(rq.Email, rq.Password);
        await _tokenEvaluator.Received().EvaluateRoles(tokenResult.Access_token);
        await _graphService.Received().GetUser(rq.Email);
    }

    [Fact]
    public async Task UserToken_EvaluateRoles_GetUser_Error()
    {
        //Arrange
        InfoByEmailCmd rq = new("email", "password");
        TokenResult tokenResult = new() { Access_token = "Access_token", Refresh_token = "Refresh_token" };
        SecurityEvaluationResult securityEvaluationResult = new() { Passed = true };
        AppException error = new("Error");

        _tokenGenerator.UserToken(rq.Email, rq.Password).Returns(tokenResult);
        _tokenEvaluator.EvaluateRoles(tokenResult.Access_token).Returns(securityEvaluationResult);
        _graphService.GetUser(rq.Email).Returns(Result.Error<User>(error));

        //Act
        var (sucesso, _, sutError) = await _sut.Handle(rq, CancellationToken.None);

        //Assert
        sucesso.Should().BeFalse();
        sutError.Should().NotBeNull().And.BeEquivalentTo(error);

        await _tokenGenerator.Received().UserToken(rq.Email, rq.Password);
        await _tokenEvaluator.Received().EvaluateRoles(tokenResult.Access_token);
        await _graphService.Received().GetUser(rq.Email);
    }

    [Fact]
    public async Task UserToken_EvaluateRoles_Error()
    {
        //Arrange
        InfoByEmailCmd rq = new("email", "password");
        TokenResult tokenResult = new() { Access_token = "Access_token", Refresh_token = "Refresh_token" };
        AppException error = new("Error");

        _tokenGenerator.UserToken(rq.Email, rq.Password).Returns(tokenResult);
        _tokenEvaluator.EvaluateRoles(tokenResult.Access_token).Returns(Result.Error<SecurityEvaluationResult>(error));

        //Act
        var (sucesso, _, sutError) = await _sut.Handle(rq, CancellationToken.None);

        //Assert
        sucesso.Should().BeFalse();
        sutError.Should().NotBeNull().And.BeEquivalentTo(new ErrorResponseBase(error.Message));

        await _tokenGenerator.Received().UserToken(rq.Email, rq.Password);
        await _tokenEvaluator.Received().EvaluateRoles(tokenResult.Access_token);
        await _graphService.DidNotReceiveWithAnyArgs().GetUser(rq.Email);
    }

    [Fact]
    public async Task UserToken_EvaluateRoles_NotPassed()
    {
        //Arrange
        InfoByEmailCmd rq = new("email", "password");
        TokenResult tokenResult = new() { Access_token = "Access_token", Refresh_token = "Refresh_token" };
        SecurityEvaluationResult securityEvaluationResult = new() { Passed = false };

        _tokenGenerator.UserToken(rq.Email, rq.Password).Returns(tokenResult);
        _tokenEvaluator.EvaluateRoles(tokenResult.Access_token).Returns(securityEvaluationResult);

        //Act
        var (sucesso, _, sutError) = await _sut.Handle(rq, CancellationToken.None);

        //Assert
        sucesso.Should().BeFalse();
        sutError.Should().NotBeNull().And.BeEquivalentTo(new ResultException($"Token not evaluated: {rq.Email}"));

        await _tokenGenerator.Received().UserToken(rq.Email, rq.Password);
        await _tokenEvaluator.Received().EvaluateRoles(tokenResult.Access_token);
        await _graphService.DidNotReceiveWithAnyArgs().GetUser(rq.Email);
    }

    [Fact]
    public async Task UserToken_AccessToken_Empty()
    {
        //Arrange
        InfoByEmailCmd rq = new("email", "password");
        TokenResult tokenResult = new() { Access_token = "", Refresh_token = "Refresh_token" };

        _tokenGenerator.UserToken(rq.Email, rq.Password).Returns(tokenResult);

        //Act
        var (sucesso, _, sutError) = await _sut.Handle(rq, CancellationToken.None);

        //Assert
        sucesso.Should().BeFalse();
        sutError.Should().NotBeNull().And.BeEquivalentTo(new ResultException($"Invalid login: {rq.Email}"));

        await _tokenGenerator.Received().UserToken(rq.Email, rq.Password);
        await _tokenEvaluator.DidNotReceiveWithAnyArgs().EvaluateRoles("");
        await _graphService.DidNotReceiveWithAnyArgs().GetUser(rq.Email);
    }

    [Fact]
    public async Task UserToken_Error()
    {
        //Arrange
        InfoByEmailCmd rq = new("email", "password");
        AppException error = new("Error");

        _tokenGenerator.UserToken(rq.Email, rq.Password).Returns(Result.Error<TokenResult>(error));

        //Act
        var (sucesso, _, sutError) = await _sut.Handle(rq, CancellationToken.None);

        //Assert
        sucesso.Should().BeFalse();
        sutError.Should().NotBeNull().And.BeEquivalentTo(error);

        await _tokenGenerator.Received().UserToken(rq.Email, rq.Password);
        await _tokenEvaluator.DidNotReceiveWithAnyArgs().EvaluateRoles("");
        await _graphService.DidNotReceiveWithAnyArgs().GetUser(rq.Email);
    }
}
