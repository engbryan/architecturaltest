using System.Diagnostics.CodeAnalysis;

namespace Bryan.Proof.Auth.Api.Client.Exceptions;

[ExcludeFromCodeCoverage]
public record ErrorResponse
{
    public ErrorResponse(AppException appException)
    {
        ErrorCode = appException.ErrorCategoryCode;
        Message = appException.Message;
    }

    public int? ErrorCode { get; init; }

    public string Message { get; init; }
}