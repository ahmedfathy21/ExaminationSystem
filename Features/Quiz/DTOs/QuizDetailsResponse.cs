using ExaminationSystem.Common.Models;

namespace ExaminationSystem.Features.Quiz.DTOs;

public class QuizDetailsResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Isntructions { get; set; }
    public int QuestionCount { get; set; }
    public int MaxAttempts { get; set; }
    public QuizStatus Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}