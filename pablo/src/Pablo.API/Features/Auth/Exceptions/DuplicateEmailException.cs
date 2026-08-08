using Pablo.API.Exceptions;

namespace Pablo.API.Features.Auth;

public sealed class DuplicateEmailException : Exception, IProblemDetailsException
{
    public int StatusCode => StatusCodes.Status409Conflict;
    public string Title => "Conflict";
    public string Detail => "This email address is already in use";
    public IReadOnlyDictionary<string, string[]>? Errors => null;

    public DuplicateEmailException()
        : base("This email address is already in use")
    {
    }
}