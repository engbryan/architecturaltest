using System.Diagnostics.CodeAnalysis;

namespace Bryan.MethodResult;

[ExcludeFromCodeCoverage]
public class ResultException : Exception
{
    public ResultException(int statusCode, string message)
        : base(message) => StatusCode = statusCode;

    public ResultException(string message)
        : base(message)
    {
    }

    public int? StatusCode { get; set; }
}

