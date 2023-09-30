using Bryan.MediatR.Extensions.Exceptions;
using Bryan.Proof.Auth.Api.Client;
using Bryan.Proof.Auth.Api.Client.Exceptions;
using Bryan.Proof.Auth.Api.Tests.TestBase;
using Bryan.Proof.Auth.Api.Tests.Utils;
using Google.Protobuf.WellKnownTypes;
using System.Net;

namespace Bryan.Proof.Auth.Api.UnitTests;

public class AppTest
{
    private static readonly TestApplication _application;
    private static readonly HttpClient _httpClient;
    private static readonly IMediator _mediator;

    static AppTest()
    {
        _application = new TestApplication();
        _httpClient = _application.CreateClient();
        _mediator = _application.Services.GetRequiredService<IMediator>();
    }

    [Fact]
    public async Task URI_INFO_BY_EMAIL_OK()
    {
        //Arrange
        InfoByEmailRq rq = new(default!, default!);
        InfoByEmailCmd cmd = new(rq.Email, rq.Password);
        _mediator.Send(cmd).Returns(new InfoRs(default!, default!, default!, default!, default!));

        //Act
        var rs = await _httpClient.PostAsync(IProofAuthService.URI_INFO_BY_EMAIL, rq.ToStringContentJson());

        //Assert
        await _mediator.Received().Send(cmd);
    }

    [Fact]
    public async Task URI_APPLICATION_ROLES_OK()
    {
        //Arrange
        //ApplicationRolesRq rq = new(default!, default!);
        ApplicationRolesCmd cmd = new();
        _mediator.Send(cmd).Returns(new ApplicationRolesRs(default!));

        //Act
        var rs = await _httpClient.GetAsync(IProofAuthService.URI_APPLICATION_ROLES);

        //Assert
        await _mediator.Received().Send(cmd);
    }

    [Fact]
    public async Task RequestDataInvalidException()
    {
        //Arrange
        InfoByEmailRq rq = new(default!, default!);
        InfoByEmailCmd cmd = new(rq.Email, rq.Password);
        RequestDataInvalidException error = new(new Dictionary<string, IEnumerable<string>> { ["FieldOne"] = (new[] { "Error Message" }) });
        _mediator.Send(cmd).Returns(Result.Error<InfoRs>(error));

        //Act
        var rs = await _httpClient.PostAsync(IProofAuthService.URI_INFO_BY_EMAIL, rq.ToStringContentJson());

        //Assert
        await _mediator.Received().Send(cmd);
        rs.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        var result = await rs.Content.ReadFromJsonAsync<IEnumerable<string>>();
        result.Should().Contain("FieldOne: Error Message");
    }

    public record TheoryDataException(Exception Error, HttpStatusCode StatusCode);
    public static TheoryData<TheoryDataException> ErrorResponse_Exceptions_Data { get; } = new TheoryData<TheoryDataException>()
    {
        new(new DataNotFoundException("TestError"), HttpStatusCode.NotFound),
        new(new AppException("TestError"), HttpStatusCode.UnprocessableEntity),
        new(new ResultException((int)HttpStatusCode.Conflict, "TestError"), HttpStatusCode.Conflict),
        new(new ResultException("TestError"), HttpStatusCode.UnprocessableEntity),
        new(new Exception(), HttpStatusCode.InternalServerError),
    };

    //[Theory]
    //[MemberData(nameof(ErrorResponse_Exceptions_Data))]
    //public async Task ErrorResponse_Exceptions(TheoryDataException data)
    //{
    //    //Arrange
    //    InfoByEmailRq rq = new(default!, default!);
    //    InfoByEmailCmd cmd = new(rq.Email, rq.Password);
    //    _mediator.Send(cmd).Returns(Result.Error<InfoRs>(data.Error));

    //    //Act
    //    var rs = await _httpClient.PostAsync(IProofAuthService.URI_INFO_BY_EMAIL, rq.ToStringContentJson());

    //    //Assert
    //    await _mediator.Received().Send(cmd);
    //    rs.StatusCode.Should().Be(data.StatusCode);
    //    var result = await rs.Content.ReadFromJsonAsync<ErrorResponseBase>();
    //    result.Should().BeEquivalentTo(new { Message = data.Error.Message! });
    //}

}

public record ErrorResponseBase(string Message);

