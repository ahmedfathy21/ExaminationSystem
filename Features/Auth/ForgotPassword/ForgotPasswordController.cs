using ExaminationSystem.Common.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExaminationSystem.Features.Auth.ForgotPassword;

[ApiController]
[Route("api/auth")]
public class ForgotPasswordController : ControllerBase
{
    private readonly IMediator _mediator;

    public ForgotPasswordController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Initiates a password reset. Sends a 6-digit code to the user's email.
    /// Always returns 200 regardless of whether the email exists (security).
    /// </summary>
    [HttpPost("forgot-password")]
    public async Task<ActionResult<ApiResponse>> ForgotPassword([FromBody] ForgotPasswordCommand command)
    {
        await _mediator.Send(command);
        return Ok(ApiResponse.Ok());
    }
}
