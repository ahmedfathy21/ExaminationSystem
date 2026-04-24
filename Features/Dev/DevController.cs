using ExaminationSystem.Common.Data;
using ExaminationSystem.Common.Models;
using ExaminationSystem.Common.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace ExaminationSystem.Features.Dev;

[ApiController]
[Route("api/dev")]
public class DevController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IHostEnvironment _env;

    public DevController(AppDbContext context, IHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    [HttpPost("seed")]
    public async Task<ActionResult<ApiResponse>> Seed()
    {
        if (!_env.IsDevelopment())
        {
            return BadRequest(ApiResponse.Fail("This endpoint is only available in development"));
        }

        if (_context.Diplomas.Any())
        {
            return Ok(new ApiResponse { Success = true, Message = "Database already seeded" });
        }

        var diploma = new Diploma
        {
            Id = Guid.NewGuid(),
            Title = "Web Development Fundamentals",
            Description = "Learn the basics of web development including HTML, CSS, and JavaScript",
            IsActive = true
        };
        _context.Diplomas.Add(diploma);

        var quiz = new Common.Models.Quiz
        {
            Id = Guid.NewGuid(),
            DiplomaId = diploma.Id,
            Title = "HTML Basics Quiz",
            Instructions = "Answer the following questions about HTML fundamentals",
            DurationMinutes = 15,
            PassScore = 70,
            MaxAttempts = 3,
            Status = QuizStatus.Published,
            PublishedAt = DateTime.UtcNow
        };
        _context.Quizzes.Add(quiz);

        var questions = new List<Question>
        {
            new()
            {
                Id = Guid.NewGuid(),
                QuizId = quiz.Id,
                Body = "What does HTML stand for?",
                Type = QuestionType.MultipleChoice,
                OrderIndex = 1
            },
            new()
            {
                Id = Guid.NewGuid(),
                QuizId = quiz.Id,
                Body = "Which tag is used for creating a paragraph?",
                Type = QuestionType.MultipleChoice,
                OrderIndex = 2
            },
            new()
            {
                Id = Guid.NewGuid(),
                QuizId = quiz.Id,
                Body = "HTML elements are case-sensitive",
                Type = QuestionType.TrueFalse,
                OrderIndex = 3
            }
        };
        _context.Questions.AddRange(questions);

        var options = new List<Option>
        {
            new() { Id = Guid.NewGuid(), QuestionId = questions[0].Id, Body = "Hyper Text Markup Language", IsCorrect = true, OrderIndex = 1 },
            new() { Id = Guid.NewGuid(), QuestionId = questions[0].Id, Body = "High Tech Modern Language", IsCorrect = false, OrderIndex = 2 },
            new() { Id = Guid.NewGuid(), QuestionId = questions[0].Id, Body = "Home Tool Markup Language", IsCorrect = false, OrderIndex = 3 },
            new() { Id = Guid.NewGuid(), QuestionId = questions[0].Id, Body = "Hyperlinks and Text Markup Language", IsCorrect = false, OrderIndex = 4 },
            new() { Id = Guid.NewGuid(), QuestionId = questions[1].Id, Body = "<p>", IsCorrect = true, OrderIndex = 1 },
            new() { Id = Guid.NewGuid(), QuestionId = questions[1].Id, Body = "<div>", IsCorrect = false, OrderIndex = 2 },
            new() { Id = Guid.NewGuid(), QuestionId = questions[1].Id, Body = "<span>", IsCorrect = false, OrderIndex = 3 },
            new() { Id = Guid.NewGuid(), QuestionId = questions[1].Id, Body = "<h1>", IsCorrect = false, OrderIndex = 4 },
            new() { Id = Guid.NewGuid(), QuestionId = questions[2].Id, Body = "True", IsCorrect = true, OrderIndex = 1 },
            new() { Id = Guid.NewGuid(), QuestionId = questions[2].Id, Body = "False", IsCorrect = false, OrderIndex = 2 }
        };
        _context.Options.AddRange(options);

        await _context.SaveChangesAsync();

        return Ok(new ApiResponse { Success = true, Message = "Database seeded successfully" });
    }
}