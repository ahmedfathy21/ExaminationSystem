using ExaminationSystem.Common.Wrappers;
using ExaminationSystem.Features.StudentProfile.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExaminationSystem.Features.StudentProfile;

[ApiController]
[Authorize]
[Route("api/student-profile")]
public class StudentProfileController : ControllerBase
{
    private readonly IMediator _mediator;

    public StudentProfileController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<StudentProfileResponse>>> GetProfile()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized(ApiResponse<StudentProfileResponse>.Fail("Unauthorized"));

        var query = new GetStudentProfileQuery(userId);
        var result = await _mediator.Send(query);

        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("dashboard")]
    public async Task<ActionResult<ApiResponse<StudentDashboardResponse>>> GetDashboard()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized(ApiResponse<StudentDashboardResponse>.Fail("Unauthorized"));

        var query = new GetStudentDashboardQuery(userId);
        var result = await _mediator.Send(query);

        return result.Success ? Ok(result) : BadRequest(result);
    }
}