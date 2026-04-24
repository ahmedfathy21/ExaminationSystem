namespace ExaminationSystem.Features.Quiz.DTOs;

public class QuizDetailsResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Instructions { get; set; }
    public int DurationMinutes { get; set; }
    public int PassScore { get; set; }
    public int MaxAttempts { get; set; }
    public int QuestionCount { get; set; }
    public int RemainingAttempts { get; set; }
    public string Status { get; set; } = string.Empty;
    public IEnumerable<AttemptSummaryDto> Attempts { get; set; } = Enumerable.Empty<AttemptSummaryDto>();
}