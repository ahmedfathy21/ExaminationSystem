using ExaminationSystem.Common.Exceptions;
using ExaminationSystem.Common.Models;
using ExaminationSystem.Common.Wrappers;
using ExaminationSystem.Features.Auth.Login;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ExaminationSystem.Features.Auth.Refresh;

[ApiController]
[Route("api/auth")]
public class RefreshController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly JwtSettings _jwtSettings;

    public RefreshController(IMediator mediator, IOptions<JwtSettings> jwtSettings)
    {
        _mediator = mediator;
        _jwtSettings = jwtSettings.Value;
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Refresh()
    {
        var refreshToken = Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(refreshToken))
        {
            throw new UnauthorizedException("Refresh token is missing from cookies.");
        }

        var command = new RefreshCommand { RefreshToken = refreshToken };
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
