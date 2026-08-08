using Pablo.API.Exceptions;

namespace Pablo.API.Features.Auth.Exceptions;

public sealed class InvalidLoginRequestException : Exception, IProblemDetailsException
{
    public const string DetailMessage = "One or more validation errors occurred.";

    public int StatusCode => StatusCodes.Status400BadRequest;
    public string Title => "Invalid login request";
    public string Detail => DetailMessage;
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public InvalidLoginRequestException(IReadOnlyDictionary<string, string[]> errors)
        : base(DetailMessage)
    {
        ArgumentNullException.ThrowIfNull(errors);
        Errors = errors;
    }

    public static InvalidLoginRequestException For(string field, string message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(field);
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        return new InvalidLoginRequestException(
            new Dictionary<string, string[]>(StringComparer.Ordinal)
            {
                [field] = [message],
            });
    }

    public static InvalidLoginRequestException MissingRequired(params string[] fields)
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

        return new InvalidLoginRequestException(errors);
    }

    public static string Required(string field) => $"The {field} field is required.";

    public static string Whitespace(string field) => $"The {field} field must not be whitespace.";

    public static string InvalidEmail() => "The Email field is not a valid email address.";
}