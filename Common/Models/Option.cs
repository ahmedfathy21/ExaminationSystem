namespace ExaminationSystem.Common.Models;

public class Option
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid QuestionId { get; set; }
    public string Body { get; set; } = string.Empty;
    public bool IsCorrect { get; set; } = false;
    public int OrderIndex { get; set; }
 
    // Navigation
    public Question Question { get; set; } = null!;
    public ICollection<Answer> Answers { get; set; } = new List<Answer>();
    
}