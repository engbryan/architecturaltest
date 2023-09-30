namespace Bryan.TokenAuth.Interfaces;

public interface ITokenGenerator
{
    Task<Result<TokenResult>> GenerateGraphToken();

    Task<Result<TokenResult>> SystemToken();

    Task<Result<TokenResult>> SystemToken(string applicationIdUri);

    Task<Result<TokenResult>> UserToken(string username, string password);

    Task<Result<TokenResult>> RefreshToken(string refreshToken);
}