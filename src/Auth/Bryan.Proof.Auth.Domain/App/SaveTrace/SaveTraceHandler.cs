using Bryan.Proof.Auth.Domain.Repositories;

namespace Bryan.Proof.Auth.Domain.App;

public class SaveTraceHandler
    : IRequestHandler<SaveTraceCmd, Result>
{
    private readonly ILogger<SaveTraceHandler> _logger;
    private readonly ITraceRepository _traceRepository;

    public SaveTraceHandler(
        ILogger<SaveTraceHandler> logger,
        ITraceRepository traceRepository
    )
    {
        _logger = logger;
        _traceRepository = traceRepository;
    }

    public Task<Result> Handle(SaveTraceCmd cmd, CancellationToken cancellationToken)
    {
        var record = new TraceRecord
        {
            Action = cmd.Action,
            DateTime = DateTime.UtcNow,
            Input = cmd.Input,
            Reference = cmd.Reference,
            User = cmd.User,
            Description = cmd.Description,
        };

        _traceRepository.Save(record);

        return Result.Success();
    }
}