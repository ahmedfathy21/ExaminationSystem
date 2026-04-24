namespace ExaminationSystem.Features.Diplomas.DTOs;

public class QuizSummaryDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int MaxAttempts { get; set; }
    public int QuestionCount { get; set; }
}