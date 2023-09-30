using System.Diagnostics.CodeAnalysis;

namespace Bryan.Proof.Auth.Api.Client.Exceptions;

[ExcludeFromCodeCoverage]
public class DataNotFoundException : AppException
{
    public DataNotFoundException(string message)
        : base(message)
    {
    }
}