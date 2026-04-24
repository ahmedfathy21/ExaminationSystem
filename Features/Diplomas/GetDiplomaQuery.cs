using ExaminationSystem.Common.Models;
using ExaminationSystem.Common.Repositories;
using ExaminationSystem.Common.Wrappers;
using ExaminationSystem.Features.Diplomas.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExaminationSystem.Features.Diplomas;

public record GetDiplomaQuery(Guid DiplomaId) : IRequest<ApiResponse<DiplomaDetailResponse>>;

public class GetDiplomaQueryHandler : IRequestHandler<GetDiplomaQuery, ApiResponse<DiplomaDetailResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetDiplomaQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<DiplomaDetailResponse>> Handle(GetDiplomaQuery request, CancellationToken cancellationToken)
    {
        var repo = _unitOfWork.Repository<Diploma>();

        var diploma = await repo.Find(d => d.Id == request.DiplomaId && d.IsActive)
            .Include(d => d.Quizzes)
            .FirstOrDefaultAsync(cancellationToken);

        if (diploma == null)
            return ApiResponse<DiplomaDetailResponse>.Fail("Diploma not found");

        var quizzes = diploma.Quizzes.Select(q => new QuizSummaryDto
        {
            Id = q.Id,
            Title = q.Title,
            Status = q.Status.ToString(),
            MaxAttempts = q.MaxAttempts,
            QuestionCount = q.Questions.Count
        });

        return ApiResponse<DiplomaDetailResponse>.Ok(new DiplomaDetailResponse
        {
            Id = diploma.Id,
            Title = diploma.Title,
            Description = diploma.Description,
            CoverImageUrl = diploma.CoverImageUrl,
            Quizzes = quizzes
        });
    }
}