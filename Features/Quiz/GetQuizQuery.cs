using ExaminationSystem.Common.Repositories;
using ExaminationSystem.Common.Wrappers;
using ExaminationSystem.Features.Quiz.DTOs;
using MediatR;
using ExaminationSystem.Common.Models;
namespace ExaminationSystem.Features.Quiz;

public record GetQuizQuery(Guid Id) : IRequest<ApiResponse<QuizDetailsResponse>>;

public class GetQuizQueryHandler : IRequestHandler<GetQuizQuery,ApiResponse<QuizDetailsResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetQuizQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
    public async Task<ApiResponse<QuizDetailsResponse>> Handle(GetQuizQuery request, CancellationToken cancellationToken)
    {
        var repo = await _unitOfWork.Repository<Common.Models.Quiz>().GetByIdAsync(request.Id).ConfigureAwait(false);
        if (repo == null)
        {
            return ApiResponse<QuizDetailsResponse>.Fail("Quiz not found");
        }

        var response = new QuizDetailsResponse
        {
            Id = repo.Id,
            Title = repo.Title,
            Isntructions = repo.Instructions,
            QuestionCount = repo.Questions.Count,
            MaxAttempts = repo.MaxAttempts,
            Status = repo.Status,
            StartDate = repo.CreatedAt,
            EndDate = repo.CreatedAt.AddMinutes(repo.DurationMinutes),
        };
        return ApiResponse<QuizDetailsResponse>.Ok(response);
    }
}