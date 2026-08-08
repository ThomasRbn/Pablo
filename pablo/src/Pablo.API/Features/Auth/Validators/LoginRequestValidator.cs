using System.ComponentModel.DataAnnotations;

namespace Pablo.API.Features.Auth.Validators;

internal static class LoginRequestValidator
{
    private static readonly EmailAddressAttribute EmailValidator = new();

    public static void Validate(LoginRequest request)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);

        AddRequiredOrWhitespace(errors, "Email", request.Email);
        AddRequiredOrWhitespace(errors, "Password", request.Password);

        if (errors.Count > 0)
        {
            throw new InvalidLoginRequestException(errors);
        }

        if (!EmailValidator.IsValid(request.Email))
        {
            throw InvalidLoginRequestException.For(
                "Email",
                InvalidLoginRequestException.InvalidEmail());
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