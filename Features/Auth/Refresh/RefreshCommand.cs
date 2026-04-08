using ExaminationSystem.Features.Auth.Login;
using MediatR;

namespace ExaminationSystem.Features.Auth.Refresh;

public class RefreshCommand : IRequest<LoginResult>
{
    public string RefreshToken { get; set; } = string.Empty;
}
