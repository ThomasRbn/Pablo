using Microsoft.AspNetCore.Identity;

using Pablo.API.Features.Auth.Exceptions;
using Pablo.API.Features.Auth.Validators;
using Pablo.API.Infrastructure.Identity;

namespace Pablo.API.Features.Auth.Services;

public sealed class AuthService(
    UserManager<AuthenticationUser> userManager,
    IJwtTokenService jwtTokenService) : IAuthService
{
    public async Task<LoginResponse> Login(
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();
        LoginRequestValidator.Validate(request);

        var user = await FindByUsernameOrEmailAsync(request.Username);
        if (user is null || await userManager.IsLockedOutAsync(user))
        {
            throw new InvalidCredentialsException();
        }

        if (!await userManager.CheckPasswordAsync(user, request.Password))
        {
            await userManager.AccessFailedAsync(user);
            throw new InvalidCredentialsException();
        }

        await userManager.ResetAccessFailedCountAsync(user);

        var accessToken = jwtTokenService.CreateAccessToken(user);
        return new LoginResponse(
            AccessToken: accessToken.Value,
            TokenType: "Bearer",
            ExpiresIn: accessToken.ExpiresInSeconds,
            Id: user.Id,
            Username: user.UserName!,
            Email: user.Email ?? string.Empty,
            DisplayName: user.DisplayName);
    }

    private async Task<AuthenticationUser?> FindByUsernameOrEmailAsync(string usernameOrEmail)
    {
        var user = await userManager.FindByNameAsync(usernameOrEmail);
        if (user is not null)
        {
            return user;
        }

        return await userManager.FindByEmailAsync(usernameOrEmail);
    }
}