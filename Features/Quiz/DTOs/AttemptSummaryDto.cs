namespace ExaminationSystem.Features.Quiz.DTOs;

public class AttemptSummaryDto
{
    public Guid Id { get; set; }
    public int AttemptNumber { get; set; }
    public int Score { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime StartedAt { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public bool IsPassed { get; set; }
}