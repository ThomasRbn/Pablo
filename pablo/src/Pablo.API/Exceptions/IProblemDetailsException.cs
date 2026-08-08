namespace Pablo.API.Exceptions;

public interface IProblemDetailsException
{
    int StatusCode { get; }
    string Title { get; }
    string Detail { get; }
    IReadOnlyDictionary<string, string[]>? Errors { get; }
}