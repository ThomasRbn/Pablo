namespace Pablo.API.Features.Auth.Types;

public record SignupRequest(string Email, string Password, string DisplayName);
public record SignupResponse(string Id, string Email, string DisplayName);