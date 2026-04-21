using ExaminationSystem.Common.Wrappers;
using ExaminationSystem.Features.Auth.Register.DTOs;
using MediatR;

namespace ExaminationSystem.Features.Auth.Register;

public record RegisterCommand(RegisterRequest Request) : IRequest<ApiResponse<RegisterResponse>>;