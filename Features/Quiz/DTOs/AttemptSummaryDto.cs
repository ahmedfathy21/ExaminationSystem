namespace ExaminationSystem.Features.Quiz.DTOs;

public class AttemptSummaryDto
{
    public int AttemptNumber { get; set; }
    public int Score { get; set; }
    public DateTime AttemptDate { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsPassed { get; set; }
    public string? Feedback { get; set; }
}