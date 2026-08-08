namespace Pablo.API.Features.Auth;

public record LoginRequest(string Username, string Password);

public record LoginResponse(
    string AccessToken,
    string TokenType,
    int ExpiresIn,
    string Id,
    string Username,
    string Email,
    string DisplayName);