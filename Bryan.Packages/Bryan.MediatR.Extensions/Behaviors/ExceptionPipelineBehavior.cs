using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bryan.MediatR.Extensions;

[ExcludeFromCodeCoverage]
public sealed class ExceptionPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly MethodInfo? _resultError;

    private readonly Type _type = typeof(TResponse);

    private readonly Type _typeResult = typeof(Result);

    private readonly ILogger<ExceptionPipelineBehavior<TRequest, TResponse>> _logger;

    public ExceptionPipelineBehavior(ILogger<ExceptionPipelineBehavior<TRequest, TResponse>> logger)
    {
        if (_type.IsGenericType)
        {
            _resultError = _typeResult.GetMethod("Error", 1, new Type[1] { typeof(Exception) });
            _resultError = _resultError!.MakeGenericMethod(_type.GetGenericArguments());
        }

        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var eventId = default(EventId);
        try
        {
            var rq = request;
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                //ObfuscationAttribute
                if (typeof(TRequest).GetCustomAttributes<JsonSerializableAttribute>().Any())
                    rq = JsonSerializer.Deserialize<TRequest>(JsonSerializer.Serialize(rq))!;
                _logger.LogInformation(eventId, "Handling request of type {TypeName} with data {@Data}", typeof(TRequest).FullName, rq);
            }
            else
            {
                _logger.LogInformation(eventId, "Handling request of type {TypeName}", typeof(TRequest).FullName);
            }

            var sp = Stopwatch.StartNew();
            var result = await next.Invoke();
            sp.Stop();

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogInformation(eventId, "Handled request of type {TypeName} with {@Data} in {Elapsed}", typeof(TRequest).FullName, result, sp.Elapsed);
            }
            else
            {
                _logger.LogInformation(eventId, "Handled request of type {TypeName} in {Elapsed}", typeof(TRequest).FullName, sp.Elapsed);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(eventId, ex, "Error on request handling");
            return (_type == _typeResult) ? ((TResponse)Convert.ChangeType(Result.Error(ex), _type)) : ((TResponse)Convert.ChangeType(_resultError!.Invoke(null, new object[1] { ex })!, _type))!;
        }
    }
}