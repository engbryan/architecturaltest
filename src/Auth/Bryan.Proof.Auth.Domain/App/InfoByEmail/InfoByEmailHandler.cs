using Bryan.TokenAuth.Interfaces;

namespace Bryan.Proof.Auth.Domain.App;

public class InfoByEmailHandler
    : IRequestHandler<InfoByEmailCmd, Result<InfoRs>>
{
    private readonly IMediator _mediator;
    private readonly ILogger<InfoByEmailHandler> _logger;
    private readonly ITokenEvaluator _evaluator;
    private readonly ITokenGenerator _generator;
    private readonly IGraphService _graphService;

    public InfoByEmailHandler(
        IMediator mediator,
        ILogger<InfoByEmailHandler> logger,
        ITokenEvaluator evaluator,
        ITokenGenerator generator,
        IGraphService graphService
    )
    {
        _mediator = mediator;
        _logger = logger;
        _evaluator = evaluator;
        _generator = generator;
        _graphService = graphService;
    }

    public virtual async Task<Result<InfoRs>> Handle(InfoByEmailCmd cmd, CancellationToken cancellationToken)
    {
        var (_, userToken, userTokenError) = await _generator.UserToken(cmd.Email, cmd.Password);
        if (userToken is null)
            return userTokenError!;

        if (string.IsNullOrEmpty(userToken.Access_token))
            return new ResultException($"Invalid login: {cmd.Email}");

        var (_, roles, rolesError) = await _evaluator.EvaluateRoles(userToken.Access_token);
        if (roles is null)
            return rolesError!;

        if (!roles.Passed)
            return new ResultException($"Token not evaluated: {cmd.Email}");

        var (_, user, userError) = await _graphService.GetUser(cmd.Email);
        if (user is null)
            return userError!;

        if (roles.Roles is not null)
            user.Roles.AddRange(roles.Roles);

        return new InfoRs(
            userToken.Access_token,
            userToken.Refresh_token,
            DateTime.UtcNow.AddMinutes(15),
            roles.ApiList?.Select(p => new ApiConfig(p.Key, (ApiEnum)p.Api)).ToList()!,
            user.ToApiClient()
        );
    }
}