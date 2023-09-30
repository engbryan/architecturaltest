using Bryan.Proof.Auth.Api.Client.Exceptions;
using Bryan.Proof.Auth.Api.UnitTests;
using Bryan.TokenAuth.Entities;
using Bryan.TokenAuth.Implementations;
using Bryan.TokenAuth.Interfaces;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office.Word;
using NSubstitute;

namespace Bryan.Proof.Auth.Api.Tests.UnitTests.Handlers;

public class ApplicationRolesHandlerTests
{
    private readonly IMediator _mediator = Substitute.For<IMediator>();
    private readonly IGraphService _graphService = Substitute.For<IGraphService>();
    private readonly ApplicationRolesHandler _sut;

    public ApplicationRolesHandlerTests()
        => _sut = new ApplicationRolesHandler(_mediator, _graphService);

    [Fact]
    public async Task GetApplicationRoles_OK()
    {
        //Arrange
        ApplicationRolesCmd rq = new();
        List<AppRole> roles = new() { new("Description", "DisplayName", "Id", true, "Value") };
        _graphService.GetApplicationRoles().Returns(roles);

        //Act
        var (sucesso, _rs, _) = await _sut.Handle(rq, CancellationToken.None);
        var rs = _rs!;

        //Assert
        sucesso.Should().BeTrue();

        rs.Should().NotBeNull();
        rs.AppRoles.Should().NotBeEmpty().And.HaveCount(roles.Count);
        rs.AppRoles[0].Description.Should().Be(roles[0].Description);
        rs.AppRoles[0].DisplayName.Should().Be(roles[0].DisplayName);
        rs.AppRoles[0].Id.Should().Be(roles[0].Id);
        rs.AppRoles[0].IsEnabled.Should().Be(roles[0].IsEnabled);
        rs.AppRoles[0].Value.Should().Be(roles[0].Value);
    }

    [Fact]
    public async Task GetApplicationRoles_Error()
    {
        //Arrange
        ApplicationRolesCmd rq = new();
        AppException error = new("Error");

        _graphService.GetApplicationRoles().Returns(Result.Error<List<AppRole>>(error));

        //Act
        var (sucesso, _, sutError) = await _sut.Handle(rq, CancellationToken.None);

        //Assert
        sucesso.Should().BeFalse();
        sutError.Should().NotBeNull().And.BeEquivalentTo(error);
    }
}
