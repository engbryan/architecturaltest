using Bryan.TokenAuth.Interfaces;

namespace Bryan.Proof.Auth.Domain.App;

public class ApplicationRolesHandler
    : IRequestHandler<ApplicationRolesCmd, Result<ApplicationRolesRs>>
{
    private readonly IMediator _mediator;
    private readonly IGraphService _graphService;

    public ApplicationRolesHandler(
        IMediator mediator,
        IGraphService graphService
    )
    {
        _mediator = mediator;
        _graphService = graphService;
    }

    public async Task<Result<ApplicationRolesRs>> Handle(ApplicationRolesCmd cmd, CancellationToken cancellationToken)
    {
        var (_, roles, rolesError) = await _graphService.GetApplicationRoles();
        if (roles is null)
            return rolesError!;

        return new ApplicationRolesRs(roles.ToApiClient());
    }
}