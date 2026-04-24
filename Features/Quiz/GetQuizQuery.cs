using ExaminationSystem.Common.Models;
using ExaminationSystem.Common.Repositories;
using ExaminationSystem.Common.Wrappers;
using ExaminationSystem.Features.Quiz.DTOs;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExaminationSystem.Features.Quiz;

public record GetQuizQuery(Guid Id) : IRequest<ApiResponse<QuizDetailsResponse>>;

public class GetQuizQueryValidator : AbstractValidator<GetQuizQuery>
{
    public GetQuizQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required");
    }   
}
public class GetQuizQueryHandler : IRequestHandler<GetQuizQuery, ApiResponse<QuizDetailsResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetQuizQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<QuizDetailsResponse>> Handle(GetQuizQuery request, CancellationToken cancellationToken)
    {
        var repo = _unitOfWork.Repository<Common.Models.Quiz>();

        var quiz = await repo.Find(q => q.Id == request.Id && q.Status == QuizStatus.Published)
            .Include(q => q.Questions)
            .FirstOrDefaultAsync(cancellationToken);

        if (quiz == null)
            return ApiResponse<QuizDetailsResponse>.Fail("Quiz not found or not published");

        var userId = Guid.NewGuid().ToString();
        var completedAttempts = quiz.Attempts
            .Where(a => a.UserId == userId && a.Status == AttemptStatus.Completed)
            .OrderBy(a => a.StartedAt)
            .ToList();

        var remainingAttempts = Math.Max(0, quiz.MaxAttempts - completedAttempts.Count);

        var attemptDtos = completedAttempts.Select((a, index) => new AttemptSummaryDto
        {
            Id = a.Id,
            AttemptNumber = index + 1,
            Score = a.Score,
            Status = a.Status.ToString(),
            StartedAt = a.StartedAt,
            SubmittedAt = a.SubmittedAt,
            IsPassed = a.Score >= quiz.PassScore
        });

        return ApiResponse<QuizDetailsResponse>.Ok(new QuizDetailsResponse
        {
            Id = quiz.Id,
            Title = quiz.Title,
            Instructions = quiz.Instructions,
            DurationMinutes = quiz.DurationMinutes,
            PassScore = quiz.PassScore,
            MaxAttempts = quiz.MaxAttempts,
            QuestionCount = quiz.Questions.Count,
            RemainingAttempts = remainingAttempts,
            Status = quiz.Status.ToString(),
            Attempts = attemptDtos
        });
    }
}