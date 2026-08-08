namespace Pablo.API.Features.Auth.Services;

public interface IAuthService
{
    Task<LoginResponse> Login(LoginRequest request);
}