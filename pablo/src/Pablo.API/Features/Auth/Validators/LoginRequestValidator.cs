using Pablo.API.Features.Auth.Exceptions;

namespace Pablo.API.Features.Auth.Validators;

internal static class LoginRequestValidator
{
    public static void Validate(LoginRequest request)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);

        AddRequiredOrWhitespace(errors, "Username", request.Username);
        AddRequiredOrWhitespace(errors, "Password", request.Password);

        if (errors.Count > 0)
        {
            throw new InvalidLoginRequestException(errors);
        }
    }

    private static void AddRequiredOrWhitespace(
        Dictionary<string, string[]> errors,
        string field,
        string? value)
    {
        if (value is null || value.Length == 0)
        {
            errors[field] = [InvalidLoginRequestException.Required(field)];
            return;
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            errors[field] = [InvalidLoginRequestException.Whitespace(field)];
        }
    }
}