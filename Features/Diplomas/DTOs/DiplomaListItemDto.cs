namespace ExaminationSystem.Features.Diplomas.DTOs;

public class DiplomaListItemDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? CoverImageUrl { get; set; }
    public int QuizCount { get; set; }
}