namespace ExaminationSystem.Common.Models;

public class Quiz :BaseEntity
{
    public Guid DiplomaId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Instructions { get; set; }
    public int DurationMinutes { get; set; }
    public int PassScore { get; set; }
    public int MaxAttempts { get; set; } = 3;
    public QuizStatus Status { get; set; } = QuizStatus.Draft;
    public DateTime? PublishedAt { get; set; }
 
    // Navigation
    public Diploma Diploma { get; set; } = null!;
    public ICollection<Question> Questions { get; set; } = new List<Question>();
    public ICollection<Attempt> Attempts { get; set; } = new List<Attempt>();
}
    
public enum QuizStatus
{
    Draft,
    Published,
}