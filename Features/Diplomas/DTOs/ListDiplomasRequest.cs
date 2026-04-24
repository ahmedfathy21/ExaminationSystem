namespace ExaminationSystem.Features.Diplomas.DTOs;

public class ListDiplomasRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}