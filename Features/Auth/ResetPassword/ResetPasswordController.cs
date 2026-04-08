using ExaminationSystem.Common.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExaminationSystem.Features.Auth.ResetPassword;

[ApiController]
[Route("api/auth")]
public class ResetPasswordController : ControllerBase
{
    private readonly IMediator _mediator;

    public ResetPasswordController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Resets the user's password using a code received via email.
    /// Revokes all existing sessions (refresh tokens) on success.
    /// </summary>
    [HttpPost("reset-password")]
    public async Task<ActionResult<ApiResponse>> ResetPassword([FromBody] ResetPasswordCommand command)
    {
        await _mediator.Send(command);
        return Ok(ApiResponse.Ok());
    }
}
