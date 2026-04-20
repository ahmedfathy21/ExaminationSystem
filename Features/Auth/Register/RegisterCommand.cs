using MediatR;

namespace ExaminationSystem.Features.Auth.Register;

public record RegisterCommand(
    string Email,
    string Password,
    string FullName,
    string Role = "Student"
) : IRequest<RegisterResponse>;

public record RegisterResponse(
    bool Success,
    string Message,
    string? UserId = null
);