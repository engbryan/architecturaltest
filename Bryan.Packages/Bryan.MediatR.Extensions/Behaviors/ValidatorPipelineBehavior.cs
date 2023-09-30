using Bryan.MediatR.Extensions.Exceptions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Bryan.MediatR.Extensions;

[ExcludeFromCodeCoverage]
public class ValidatorPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
where TRequest : IRequest<TResponse>
{
    private readonly ILogger<ValidatorPipelineBehavior<TRequest, TResponse>> _logger;
    //private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly IServiceProvider? _serviceProvider;
    private static readonly Type _type;
    private static readonly MethodInfo? _resultError;

    internal static class ValidationBehaviorMeta
    {
        internal static readonly Type _typeResult = typeof(Result);

        internal static readonly MethodInfo _genericErrorMethod = _typeResult.GetMethod("Error", 1, new Type[1] { typeof(Exception) })!;
    }

    static ValidatorPipelineBehavior()
    {
        _type = typeof(TResponse);
        if (_type.IsGenericType)
        {
            _resultError = ValidationBehaviorMeta._genericErrorMethod.MakeGenericMethod(_type.GetGenericArguments());
        }
    }

    public ValidatorPipelineBehavior(
        ILogger<ValidatorPipelineBehavior<TRequest, TResponse>> logger,
        //IEnumerable<IValidator<TRequest>> validators,
        IServiceProvider serviceProvider
    )
    {
        _serviceProvider = serviceProvider;
        //_validators = validators;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var genericTypeName = request.GetGenericTypeName();
        _logger.LogInformation("----- Validating command {CommandType}", genericTypeName);

        var _validators = _serviceProvider!.GetServices<AbstractValidator<TRequest>>();
        if (!_validators.Any())
            return await next.Invoke();

        var context = new ValidationContext<TRequest>(request);
        var results = await Task.WhenAll(_validators.Select(x => x.ValidateAsync(context, cancellationToken)));
        var validations = results
            .SelectMany(p => p.Errors)
            .Where(x => x != null)
            .GroupBy(x => x.PropertyName)
            .ToDictionary(
                (IGrouping<string, ValidationFailure> g) => g.Key,
                (IGrouping<string, ValidationFailure> g) => g.Select(s => s.ErrorMessage)
            );

        if (!validations.Any())
            return await next.Invoke();

        _logger.LogWarning("Validation errors - {CommandType} - Command: {@Command} - Errors: {@ValidationErrors}", genericTypeName, request, validations);

        RequestDataInvalidException ex = new(validations);

        if (_type != ValidationBehaviorMeta._typeResult)
        {
            return await Task.FromResult((TResponse)Convert.ChangeType(_resultError!.Invoke(null, new object[1] { ex }), _type)!);
        }

        return await Task.FromResult((TResponse)Convert.ChangeType(Result.Error(ex), _type));
    }
}