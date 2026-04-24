using ExaminationSystem.Common.Models;
using ExaminationSystem.Common.Wrappers;
using ExaminationSystem.Features.StudentProfile.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ExaminationSystem.Features.StudentProfile;

public record GetStudentProfileQuery(string UserId) : IRequest<ApiResponse<StudentProfileResponse>>;
public class GetStudentProfileQueryHandler : IRequestHandler<GetStudentProfileQuery, ApiResponse<StudentProfileResponse>>
{
    private readonly UserManager<AppUser> _userManager;

    public GetStudentProfileQueryHandler(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ApiResponse<StudentProfileResponse>> Handle(GetStudentProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
            return ApiResponse<StudentProfileResponse>.Fail("User not found");

        return ApiResponse<StudentProfileResponse>.Ok(new StudentProfileResponse
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            Role = user.Role.ToString()
        });
    }
}