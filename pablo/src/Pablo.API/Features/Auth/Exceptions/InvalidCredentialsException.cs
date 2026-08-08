using Pablo.API.Exceptions;

namespace Pablo.API.Features.Auth;

public sealed class InvalidCredentialsException : Exception, IProblemDetailsException
{
    public int StatusCode => StatusCodes.Status401Unauthorized;
    public string Title => "Unauthorized";
    public string Detail => "Invalid email or password";
    public IReadOnlyDictionary<string, string[]>? Errors => null;

    public InvalidCredentialsException()
        : base("Invalid email or password")
    {
    }
}