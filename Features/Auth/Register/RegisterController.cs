using ExaminationSystem.Common.Wrappers;
using ExaminationSystem.Features.Auth.Register;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExaminationSystem.Features.Auth.Register;

[ApiController]
[Route("api/[controller]")]
public class RegisterController : ControllerBase
{
    private readonly IMediator _mediator;

    public RegisterController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<RegisterResponse>>> Register([FromBody] RegisterCommand command)
    {
        var result = await _mediator.Send(command);
        
        if (!result.Success)
        {
            return BadRequest(ApiResponse<RegisterResponse>.Fail(result.Message));
        }

        return Ok(ApiResponse<RegisterResponse>.Ok(result));
    }
}