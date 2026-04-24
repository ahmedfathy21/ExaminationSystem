using ExaminationSystem.Common.Wrappers;
using ExaminationSystem.Features.Quiz.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExaminationSystem.Features.Quiz;

[ApiController]
[Authorize]
[Route("api/quiz")]
public class QuizController : ControllerBase
{
    private readonly IMediator _mediator;

    public QuizController(IMediator mediator) => _mediator = mediator;

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<QuizDetailsResponse>>> GetQuiz(Guid id)
    {
        var result = await _mediator.Send(new GetQuizQuery(id));
        return result.Success ? Ok(result) : NotFound(result);
    }
}