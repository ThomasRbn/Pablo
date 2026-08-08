using Pablo.API.Infrastructure.Identity;

namespace Pablo.API.Features.Auth.Services;

public interface IJwtTokenService
{
    AccessToken CreateAccessToken(AuthenticationUser user);
}

public readonly record struct AccessToken(string Value, int ExpiresInSeconds);