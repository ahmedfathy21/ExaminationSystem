namespace ExaminationSystem.Common.Models;

public class Attempt : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid QuizId { get; set; }
    public int Score { get; set; } = 0;
    public int TotalQuestions { get; set; } = 0;
    public AttemptStatus Status { get; set; } = AttemptStatus.InProgress;
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? SubmittedAt { get; set; }
 
    // Navigation
    public AppUser? User { get; set; }
    public Quiz Quiz { get; set; } = null!;
    public ICollection<Answer> Answers { get; set; } = new List<Answer>();
    
}

public enum AttemptStatus
{
    InProgress,
    Completed,
    Failed,
}