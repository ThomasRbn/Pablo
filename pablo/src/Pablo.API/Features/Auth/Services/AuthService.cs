using Microsoft.AspNetCore.Identity;

using Pablo.API.Features.Auth.Validators;
using Pablo.API.Infrastructure.Identity;

namespace Pablo.API.Features.Auth.Services;

public sealed class AuthService(UserManager<AuthenticationUser> userManager) : IAuthService
{
    public async Task<LoginResponse> Login(LoginRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        LoginRequestValidator.Validate(request);

        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null || !await userManager.CheckPasswordAsync(user, request.Password))
        {
            throw new InvalidCredentialsException();
        }

        return new LoginResponse(user.Id, user.Email!, user.DisplayName);
    }
}