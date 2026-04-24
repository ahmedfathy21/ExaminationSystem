using ExaminationSystem.Common.Models;
using ExaminationSystem.Common.Repositories;
using ExaminationSystem.Common.Wrappers;
using ExaminationSystem.Features.Diplomas.DTOs;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExaminationSystem.Features.Diplomas;

public record ListDiplomasQuery(ListDiplomasRequest Request) : IRequest<PagedResponse<DiplomaListItemDto>>;

public class ListDiplomasQueryValidator : AbstractValidator<ListDiplomasQuery>
{
    public ListDiplomasQueryValidator()
    {
        RuleFor(x => x.Request.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page must be at least 1");

        RuleFor(x => x.Request.PageSize)
            .InclusiveBetween(1, 50).WithMessage("PageSize must be between 1 and 50");
    }
}

public class ListDiplomasQueryHandler : IRequestHandler<ListDiplomasQuery, PagedResponse<DiplomaListItemDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ListDiplomasQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResponse<DiplomaListItemDto>> Handle(ListDiplomasQuery request, CancellationToken cancellationToken)
    {
        var repo = _unitOfWork.Repository<Diploma>();
        
        var query = repo.Find(d => d.IsActive)
            .OrderByDescending(d => d.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.Request.Page - 1) * request.Request.PageSize)
            .Take(request.Request.PageSize)
            .Select(d => new DiplomaListItemDto
            {
                Id = d.Id,
                Title = d.Title,
                Description = d.Description,
                CoverImageUrl = d.CoverImageUrl,
                QuizCount = d.Quizzes.Count
            })
            .ToListAsync(cancellationToken);

        return new PagedResponse<DiplomaListItemDto>
        {
            Data = items,
            TotalCount = totalCount,
            Page = request.Request.Page,
            PageSize = request.Request.PageSize
        };
    }
}