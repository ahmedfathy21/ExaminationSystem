using ExaminationSystem.Common.Wrappers;
using ExaminationSystem.Features.Auth.Register;
using ExaminationSystem.Features.Auth.Register.DTOs;
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
    public async Task<ActionResult<ApiResponse<RegisterResponse>>> Register([FromBody] RegisterRequest request)
    {
        var command = new RegisterCommand(request);
        var result = await _mediator.Send(command);
        
        return result.Success ? Ok(result) : BadRequest(result);
    }
}