namespace ExaminationSystem.Features.Diplomas.DTOs;

public class DiplomaDetailResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? CoverImageUrl { get; set; }
    public IEnumerable<QuizSummaryDto> Quizzes { get; set; } = Enumerable.Empty<QuizSummaryDto>();
}