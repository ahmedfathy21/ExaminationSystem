using ExaminationSystem.Common.Models;
using ExaminationSystem.Common.Repositories;
using ExaminationSystem.Common.Wrappers;
using ExaminationSystem.Features.StudentProfile.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ExaminationSystem.Features.StudentProfile;

public record GetStudentDashboardQuery(string UserId) : IRequest<ApiResponse<StudentDashboardResponse>>;

public class GetStudentDashboardQueryHandler : IRequestHandler<GetStudentDashboardQuery, ApiResponse<StudentDashboardResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<AppUser> _userManager;

    public GetStudentDashboardQueryHandler(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task<ApiResponse<StudentDashboardResponse>> Handle(GetStudentDashboardQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
            return ApiResponse<StudentDashboardResponse>.Fail("User not found");

        var attempts = await _unitOfWork.Repository<Attempt>()
            .Find(a => a.UserId == request.UserId)
            .Include(a => a.Quiz)
            .ThenInclude(q => q.Diploma)
            .ToListAsync(cancellationToken);

        var enrolledDiplomaIds = attempts
            .Select(a => a.Quiz.DiplomaId)
            .Distinct()
            .ToList();

        var diplomas = new List<Diploma>();
        if (enrolledDiplomaIds.Any())
        {
            diplomas = await _unitOfWork.Repository<Diploma>()
                .Find(d => enrolledDiplomaIds.Contains(d.Id))
                .Include(d => d.Quizzes.Where(q => q.Status == QuizStatus.Published))
                .ToListAsync(cancellationToken);
        }

        var attemptsByQuizId = attempts.GroupBy(a => a.QuizId).ToDictionary(g => g.Key, g => g.ToList());

        var enrolledDiplomas = new List<EnrolledDiplomaDto>();
        foreach (var diploma in diplomas)
        {
            var publishedQuizzes = diploma.Quizzes.ToList();
            var diplomaQuizzes = new List<DiplomaQuizDto>();
            var completedCount = 0;
            var inProgressCount = 0;
            var notStartedCount = 0;

            foreach (var quiz in publishedQuizzes)
            {
                var quizAttempts = attemptsByQuizId.TryGetValue(quiz.Id, out var value) ? value : new List<Attempt>();
                var hasCompletedOrFailed = quizAttempts.Any(a => a.Status == AttemptStatus.Completed || a.Status == AttemptStatus.Failed);
                var hasInProgress = quizAttempts.Any(a => a.Status == AttemptStatus.InProgress);

                var status = hasCompletedOrFailed ? "Completed" : hasInProgress ? "InProgress" : "NotStarted";

                if (status == "Completed") completedCount++;
                else if (status == "InProgress") inProgressCount++;
                else notStartedCount++;

                var bestScore = quizAttempts
                    .Where(a => a.Status == AttemptStatus.Completed || a.Status == AttemptStatus.Failed)
                    .Max(a => (int?)a.Score) ?? 0;

                var lastAttemptAt = quizAttempts.Any() ? quizAttempts.Max(a => a.StartedAt) : (DateTime?)null;

                diplomaQuizzes.Add(new DiplomaQuizDto
                {
                    QuizId = quiz.Id,
                    Title = quiz.Title,
                    Status = status,
                    BestScore = bestScore,
                    TotalQuestions = quizAttempts.Any() ? quizAttempts.First().TotalQuestions : quiz.Questions.Count,
                    PassScore = quiz.PassScore,
                    LastAttemptAt = lastAttemptAt
                });
            }

            var totalPublished = publishedQuizzes.Count;
            var progressPercentage = totalPublished > 0 ? (double)completedCount / totalPublished * 100 : 0;

            enrolledDiplomas.Add(new EnrolledDiplomaDto
            {
                Id = diploma.Id,
                Title = diploma.Title,
                Description = diploma.Description ?? string.Empty,
                CoverImageUrl = diploma.CoverImageUrl ?? string.Empty,
                TotalQuizzes = totalPublished,
                CompletedQuizzes = completedCount,
                InProgressQuizzes = inProgressCount,
                NotStartedQuizzes = notStartedCount,
                ProgressPercentage = Math.Round(progressPercentage, 1),
                Quizzes = diplomaQuizzes
            });
        }

        var recentAttempts = attempts
            .OrderByDescending(a => a.StartedAt)
            .Take(10)
            .Select(a => new RecentAttemptDto
            {
                AttemptId = a.Id,
                QuizId = a.QuizId,
                QuizTitle = a.Quiz.Title,
                DiplomaTitle = a.Quiz.Diploma.Title,
                Status = a.Status.ToString(),
                Score = a.Score,
                TotalQuestions = a.TotalQuestions,
                StartedAt = a.StartedAt,
                SubmittedAt = a.SubmittedAt
            })
            .ToList();

        var submittedAttempts = attempts.Where(a => a.Status == AttemptStatus.Completed || a.Status == AttemptStatus.Failed).ToList();
        var uniqueQuizIdsAttempted = attempts.Select(a => a.QuizId).Distinct().Count();
        var inProgressOnlyCount = attemptsByQuizId.Count(kvp =>
            kvp.Value.Any(a => a.Status == AttemptStatus.InProgress) &&
            kvp.Value.All(a => a.Status != AttemptStatus.Completed && a.Status != AttemptStatus.Failed));

        var passedCount = 0;
        var failedCount = 0;
        var allScores = new List<double>();

        foreach (var quizAttemptGroup in submittedAttempts.GroupBy(a => a.QuizId))
        {
            var bestScore = quizAttemptGroup.Max(a => a.Score);
            var quiz = quizAttemptGroup.First().Quiz;
            if (bestScore >= quiz.PassScore)
                passedCount++;
            else
                failedCount++;

            allScores.AddRange(quizAttemptGroup.Select(a => (double)a.Score / a.TotalQuestions * 100));
        }

        var averageScore = allScores.Any() ? Math.Round(allScores.Average(), 1) : 0;
        var totalPassedOrFailed = passedCount + failedCount;
        var passRate = totalPassedOrFailed > 0 ? Math.Round((double)passedCount / totalPassedOrFailed * 100, 1) : 0;

        var statistics = new DashboardStatisticsDto
        {
            TotalDiplomas = enrolledDiplomas.Count,
            TotalQuizzesAttempted = uniqueQuizIdsAttempted,
            TotalQuizzesPassed = passedCount,
            TotalQuizzesFailed = failedCount,
            InProgressQuizzes = inProgressOnlyCount,
            AverageScore = averageScore,
            PassRate = passRate
        };

        var profile = new DashboardProfileDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty
        };

        return ApiResponse<StudentDashboardResponse>.Ok(new StudentDashboardResponse
        {
            Profile = profile,
            EnrolledDiplomas = enrolledDiplomas,
            RecentAttempts = recentAttempts,
            Statistics = statistics
        });
    }
}
