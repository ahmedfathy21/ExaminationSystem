namespace ExaminationSystem.Common.Models;

public class Question : BaseEntity
{
    public Guid QuizId { get; set; }
    public string Body { get; set; } = string.Empty;
    public QuestionType Type { get; set; } = QuestionType.MultipleChoice;
    public int OrderIndex { get; set; }
 
    // Navigation
    public Quiz Quiz { get; set; } = null!;
    public ICollection<Option> Options { get; set; } = new List<Option>();
    public ICollection<Answer> Answers { get; set; } = new List<Answer>();
    
}

public enum QuestionType
{
    MultipleChoice,
    TrueFalse,
}