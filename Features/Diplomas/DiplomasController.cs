using ExaminationSystem.Common.Wrappers;
using ExaminationSystem.Features.Diplomas.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExaminationSystem.Features.Diplomas;

[ApiController]
[Route("api/diplomas")]
public class DiplomasController : ControllerBase
{
    private readonly IMediator _mediator;

    public DiplomasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<DiplomaListItemDto>>> ListDiplomas([FromQuery] ListDiplomasRequest request)
    {
        var query = new ListDiplomasQuery(request);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
} 