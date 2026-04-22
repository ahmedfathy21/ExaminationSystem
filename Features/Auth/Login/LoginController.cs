using ExaminationSystem.Common.Wrappers;
using ExaminationSystem.Features.Auth.Login;
using ExaminationSystem.Features.Auth.Login.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExaminationSystem.Features.Auth.Login;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly IMediator _mediator;

    public LoginController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest request)
    {
        var command = new LoginCommand(request);
        var result = await _mediator.Send(command);

        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<ApiResponse<RefreshTokenResponse>>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var command = new RefreshTokenCommand(request);
        var result = await _mediator.Send(command);

        return result.Success ? Ok(result) : BadRequest(result);
    }
}
