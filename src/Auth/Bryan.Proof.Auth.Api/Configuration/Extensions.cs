using Bryan.MediatR.Extensions.Exceptions;
using Bryan.Proof.Auth.Api.Client.Exceptions;
using System.Net;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace Bryan.Proof.Auth.Api.Configuration;

public static partial class Extensions
{
    public static async Task<IResult> SendCommand<T>(this IMediator mediator, IRequest<Result<T>> request, int statusCode = 200)
        => await mediator.Send(request) switch
        {
            (true, var result, _) => new StatusCodeResult<T>(statusCode, result),
            var (_, _, error) => HandleErrorApp(error!)
        };

    private static IResult HandleErrorApp(Exception error)
        => error switch
        {
            DataNotFoundException e => new StatusCodeResult<ErrorResponse>((int)HttpStatusCode.NotFound, new ErrorResponse(e)),
            AppException e => new StatusCodeResult<ErrorResponse>((int)HttpStatusCode.Unauthorized, new ErrorResponse(e)),
            _ => HandleErrorBase(error)
        };

    private static IResult HandleErrorBase(Exception error)
        => error switch
        {
            RequestDataInvalidException e => Results.BadRequest(e.Errors),
            ResultException e => e.StatusCode.HasValue switch
            {
                true => new StatusCodeResult<ErrorResponse>(e.StatusCode.Value, new ErrorResponse(new(e.Message))),
                _ => new StatusCodeResult<ErrorResponse>((int)HttpStatusCode.Unauthorized, new ErrorResponse(new(e.Message))),
            },
#if DEBUG
            Exception e => new StatusCodeResult<ErrorResponse>((int)HttpStatusCode.InternalServerError, new(new(e.Message)))
#else
            _ => new StatusCodeResult<ErrorResponse>((int)HttpStatusCode.InternalServerError, null)
#endif
        };

    private readonly record struct StatusCodeResult<T>(int StatusCode, T? Value) : IResult
    {
        public Task ExecuteAsync(HttpContext httpContext)
        {
            httpContext.Response.StatusCode = StatusCode;
            return Value is null
                ? Task.CompletedTask
                : httpContext.Response.WriteAsJsonAsync(Value, Value.GetType(), options: null, contentType: "application/json");
        }
    }
}