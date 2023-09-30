using System.Diagnostics.CodeAnalysis;

namespace Bryan.Proof.Auth.Api.Client.Exceptions;

[ExcludeFromCodeCoverage]
public class AppException : Exception
{
    public AppException(int code, string message)
        : base(message) => ErrorCategoryCode = code;

    public AppException(string message)
        : base(message)
    {
    }

    public int? ErrorCategoryCode { get; set; }
}