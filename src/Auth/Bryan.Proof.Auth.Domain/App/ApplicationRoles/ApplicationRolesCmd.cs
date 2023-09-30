namespace Bryan.Proof.Auth.Domain.App;

public record ApplicationRolesCmd() : IRequest<Result<ApplicationRolesRs>>;