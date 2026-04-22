using ExaminationSystem.Common.Wrappers;
using ExaminationSystem.Features.Auth.VerifyAccount;
using ExaminationSystem.Features.Auth.VerifyAccount.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExaminationSystem.Features.Auth.VerifyAccount;

[ApiController]
[Route("api/[controller]")]
public class VerifyAccountController : ControllerBase
{
    private readonly IMediator _mediator;

    public VerifyAccountController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("send-verification-otp")]
    public async Task<ActionResult<ApiResponse<VerifyAccountResponse>>> SendVerificationOtp([FromBody] VerifyAccountRequest request)
    {
        var command = new VerifyAccountCommand(request);
        var result = await _mediator.Send(command);

        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("verify-otp")]
    public async Task<ActionResult<ApiResponse<VerifyOtpResponse>>> VerifyOtp([FromBody] VerifyOtpRequest request)
    {
        var command = new VerifyOtpCommand(request);
        var result = await _mediator.Send(command);

        return result.Success ? Ok(result) : BadRequest(result);
    }
}