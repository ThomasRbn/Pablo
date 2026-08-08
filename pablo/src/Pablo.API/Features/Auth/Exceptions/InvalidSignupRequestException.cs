using Pablo.API.Exceptions;

namespace Pablo.API.Features.Auth;

public sealed class InvalidSignupRequestException : Exception, IProblemDetailsException
{
    public const string DetailMessage = "One or more validation errors occurred.";

    public int StatusCode => StatusCodes.Status400BadRequest;
    public string Title => "Invalid signup request";
    public string Detail => DetailMessage;
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public InvalidSignupRequestException(IReadOnlyDictionary<string, string[]> errors)
        : base(DetailMessage)
    {
        ArgumentNullException.ThrowIfNull(errors);
        Errors = errors;
    }

    public static InvalidSignupRequestException For(string field, string message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(field);
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        return new InvalidSignupRequestException(
            new Dictionary<string, string[]>(StringComparer.Ordinal)
            {
                [field] = [message],
            });
    }

    public static InvalidSignupRequestException MissingRequired(params string[] fields)
    {
        ArgumentNullException.ThrowIfNull(fields);
        if (fields.Length == 0)
        {
            throw new ArgumentException("At least one field is required.", nameof(fields));
        }

        var errors = fields.ToDictionary(
            field => field,
            field => new[] { Required(field) },
            StringComparer.Ordinal);

        return new InvalidSignupRequestException(errors);
    }

    public static string Required(string field) => $"The {field} field is required.";

    public static string Whitespace(string field) => $"The {field} field must not be whitespace.";

    public static string InvalidEmail() => "The Email field is not a valid email address.";

    public static string PasswordTooShort() => "Password must be at least 8 characters.";

    public static string PasswordMissingUppercase() => "Password must contain an uppercase letter.";

    public static string PasswordMissingLowercase() => "Password must contain a lowercase letter.";

    public static string PasswordMissingSymbol() => "Password must contain a symbol.";

    public static string PasswordMissingDigit() => "Password must contain a digit.";
}