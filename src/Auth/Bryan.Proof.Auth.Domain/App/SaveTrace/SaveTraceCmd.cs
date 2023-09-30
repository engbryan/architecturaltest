namespace Bryan.Proof.Auth.Domain.App;

public record SaveTraceCmd(string User, ActionEnum Action, string Reference, object Input, string Description) : IRequest<Result>;