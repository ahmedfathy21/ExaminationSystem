using ExaminationSystem.Common.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExaminationSystem.Features.Auth.Verify;

[ApiController]
[Route("api/auth")]
public class VerifyController : ControllerBase
{
    private readonly IMediator _mediator;

    public VerifyController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("verify")]
    public async Task<ActionResult<ApiResponse>> Verify([FromBody] VerifyCommand command)
    {
        await _mediator.Send(command);
        return Ok(ApiResponse.Ok());
    }
}
