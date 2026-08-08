using Microsoft.AspNetCore.Mvc;

using Pablo.API.Features.Auth.Services;

namespace Pablo.API.Features.Auth;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest? request)
    {
        if (request is null)
        {
            throw InvalidLoginRequestException.MissingRequired("Email", "Password");
        }

        var result = await authService.Login(request);
        return Ok(result);
    }
}