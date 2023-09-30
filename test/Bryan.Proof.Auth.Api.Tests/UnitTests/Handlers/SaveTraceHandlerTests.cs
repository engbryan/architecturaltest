using Bryan.Proof.Auth.Api.Client.Exceptions;
using Bryan.Proof.Auth.Api.UnitTests;
using Bryan.Proof.Auth.Domain.Entities;
using Bryan.Proof.Auth.Domain.Repositories;
using Bryan.TokenAuth.Entities;
using Bryan.TokenAuth.Implementations;
using Bryan.TokenAuth.Interfaces;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office.Word;
using NSubstitute;

namespace Bryan.Proof.Auth.Api.Tests.UnitTests.Handlers;

public class SaveTraceHandlerTests
{
    private readonly ILogger<SaveTraceHandler> _logger = Substitute.For<ILogger<SaveTraceHandler>>();
    private readonly ITraceRepository _traceRepository = Substitute.For<ITraceRepository>();
    private readonly SaveTraceHandler _sut;

    public SaveTraceHandlerTests()
        => _sut = new SaveTraceHandler(_logger, _traceRepository);

    [Fact]
    public async Task Save_OK()
    {
        //Arrange
        SaveTraceCmd rq = new("User", Dynamo.Abstractions.Enums.ActionEnum.Attachment, "Ref", new { Tmp="tmp"}, "Description");
        
        //Act
        var sucesso = await _sut.Handle(rq, CancellationToken.None);

        //Assert
        sucesso.IsSuccess.Should().BeTrue();

        _traceRepository.Received().Save(Arg.Is<TraceRecord>(p =>
            p.Action == rq.Action &&
            DateTime.UtcNow.Subtract(p.DateTime).TotalSeconds <= 60 &&
            p.Input == rq.Input &&
            p.Reference == rq.Reference &&
            p.User == rq.User &&
            p.Description == rq.Description)
        );
    }
}
