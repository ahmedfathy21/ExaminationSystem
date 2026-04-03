namespace ExaminationSystem.Common.Models;

public class Diploma : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? CoverImageUrl { get; set; }
    public bool IsActive { get; set; } = true;
 
    // Navigation
    public ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
    
}