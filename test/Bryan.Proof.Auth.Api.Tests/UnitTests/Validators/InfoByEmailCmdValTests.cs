namespace Bryan.Proof.Auth.Api.Tests.UnitTests.Validators;

public class InfoByEmailCmdValTests
{
    public static TheoryData<InfoByEmailCmd> ValidRequest { get; } = new TheoryData<InfoByEmailCmd>
    {
        new("email@host.com", "password"),
    };

    [Theory]
    [MemberData(nameof(ValidRequest))]
    public async Task Valid(InfoByEmailCmd rq)
    {
        //Arrange
        var sut = new InfoByEmailCmdValidator();

        //Act
        var rs = await sut.ValidateAsync(rq);

        //Assert
        rs.IsValid.Should().BeTrue();
    }

    public static TheoryData<InfoByEmailCmd> InvalidRequest { get; } = new TheoryData<InfoByEmailCmd>
    {
        new("", ""),
        new("", "password"),
        new("email", "password"),
        new("email@host.com", ""),
    };

    [Theory]
    [MemberData(nameof(InvalidRequest))]
    public async Task Invalid(InfoByEmailCmd rq)
    {
        //Arrange
        var sut = new InfoByEmailCmdValidator();

        //Act
        var rs = await sut.ValidateAsync(rq);

        //Assert
        rs.IsValid.Should().BeFalse();
    }
}
