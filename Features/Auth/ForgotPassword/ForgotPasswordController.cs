using ExaminationSystem.Common.Wrappers;
using ExaminationSystem.Features.Auth.ForgotPassword;
using ExaminationSystem.Features.Auth.ForgotPassword.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExaminationSystem.Features.Auth.ForgotPassword;

[ApiController]
[Route("api/[controller]")]
public class ForgotPasswordController : ControllerBase
{
    private readonly IMediator _mediator;

    public ForgotPasswordController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("forgot-password")]
    public async Task<ActionResult<ApiResponse<ForgotPasswordResponse>>> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        var command = new ForgotPasswordCommand(request);
        var result = await _mediator.Send(command);

        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("reset-password")]
    public async Task<ActionResult<ApiResponse<ResetPasswordResponse>>> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var command = new ResetPasswordCommand(request);
        var result = await _mediator.Send(command);

        return result.Success ? Ok(result) : BadRequest(result);
    }
}