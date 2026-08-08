namespace Pablo.API.Features.Auth;

public record LoginRequest(string Email, string Password);

public record LoginResponse(
    string AccessToken,
    string TokenType,
    int ExpiresIn,
    string Id,
    string Email,
    string DisplayName);