using ExaminationSystem.Common.Models;
using ExaminationSystem.Common.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ExaminationSystem.Features.Auth.Login;

[ApiController]
[Route("api/auth")]
public class LoginController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly JwtSettings _jwtSettings;

    public LoginController(IMediator mediator, IOptions<JwtSettings> jwtSettings)
    {
        _mediator = mediator;
        _jwtSettings = jwtSettings.Value;
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginCommand command)
    {
        var result = await _mediator.Send(command);

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
            SameSite = SameSiteMode.Strict,
            Secure = true // Set to true in production
        };

        Response.Cookies.Append("refreshToken", result.RefreshToken, cookieOptions);

        var response = new LoginResponse
        {
            AccessToken = result.AccessToken
        };

        return Ok(ApiResponse<LoginResponse>.Ok(response));
    }
}
