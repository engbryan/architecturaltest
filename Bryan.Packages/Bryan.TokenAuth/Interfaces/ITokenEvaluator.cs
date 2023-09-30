namespace Bryan.TokenAuth.Interfaces;

public interface ITokenEvaluator
{
    Task<Result<SecurityEvaluationResult>> EvaluateRoles(string token);

    Task<Result<SecurityEvaluationResult>> EvaluateUser(string email, Guid marketCode);
}