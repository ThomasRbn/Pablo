namespace Pablo.API.Features.Auth;

public record LoginRequest(string Email, string Password);
public record LoginResponse(string Id, string Email, string DisplayName);