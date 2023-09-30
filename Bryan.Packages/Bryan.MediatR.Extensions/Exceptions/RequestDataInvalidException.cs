namespace Bryan.MediatR.Extensions.Exceptions;

public class RequestDataInvalidException : ApplicationException
{
    public IEnumerable<string> Errors { get; }

    public RequestDataInvalidException(IDictionary<string, IEnumerable<string>> messages)
        : base("Invalid Data") => Errors = messages.Select((KeyValuePair<string, IEnumerable<string>> m)
            => m.Key + ": " + string.Join(", ", m.Value));
}