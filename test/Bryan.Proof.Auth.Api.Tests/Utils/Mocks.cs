using Bryan.TokenAuth.Entities;

namespace Bryan.Proof.Auth.Api.Tests.Utils;

internal static class Mocks
{
    public static SecurityEvaluationResult SecurityEvaluationResult() => new() { Passed = true };

    public static TokenResult TokenResult() => new() { Access_token = "Access_token" };
}