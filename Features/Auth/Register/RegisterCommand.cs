using ExaminationSystem.Features.Auth.Register.DTOs;
using MediatR;

namespace ExaminationSystem.Features.Auth.Register;

public record RegisterCommand(RegisterRequest Request) : IRequest<RegisterResponse>;