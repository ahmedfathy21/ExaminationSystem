namespace ExaminationSystem.Common.Models;

public class Answer
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AttemptId { get; set; }
    public Guid QuestionId { get; set; }
    public Guid SelectedOptionId { get; set; }
    public bool IsCorrect { get; set; } = false;
    public DateTime AnsweredAt { get; set; } = DateTime.UtcNow;
 
    // Navigation
    public Attempt Attempt { get; set; } = null!;
    public Question Question { get; set; } = null!;
    public Option SelectedOption { get; set; } = null!;
}