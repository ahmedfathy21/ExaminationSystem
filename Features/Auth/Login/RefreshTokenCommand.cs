using ExaminationSystem.Common.Wrappers;
using ExaminationSystem.Features.Auth.Login.DTOs;
using MediatR;

namespace ExaminationSystem.Features.Auth.Login;

public record RefreshTokenCommand(RefreshTokenRequest Request) : IRequest<ApiResponse<RefreshTokenResponse>>;