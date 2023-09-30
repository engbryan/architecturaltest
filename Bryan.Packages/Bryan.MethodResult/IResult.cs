namespace Bryan.MethodResult;

public interface IResult
{
    Exception? Exception { get; }
    bool IsSuccess { get; }
}