using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExaminationSystem.Features.Quiz;
[ApiController]
[Route("api/quiz")]
public class QuizController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public QuizController(IMediator mediator) => _mediator = mediator;
        
    public async Task<IActionResult> GetQuiz(Guid id) => Ok(await _mediator.Send(new GetQuizQuery(id)));
}