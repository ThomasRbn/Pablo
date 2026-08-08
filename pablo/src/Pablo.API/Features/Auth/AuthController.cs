using Microsoft.AspNetCore.Mvc;

using Pablo.API.Features.Auth.Exceptions;
using Pablo.API.Features.Auth.Services;

namespace Pablo.API.Features.Auth;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(
        [FromBody] LoginRequest? request,
        CancellationToken cancellationToken)
    {
        if (request is null)
        {
            throw InvalidLoginRequestException.MissingRequired("Username", "Password");
        }

        var result = await authService.Login(request, cancellationToken);
        return Ok(result);
    }
}