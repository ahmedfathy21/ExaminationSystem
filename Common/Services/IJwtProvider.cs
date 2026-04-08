namespace ExaminationSystem.Common.Services;

using ExaminationSystem.Common.Models;

public interface IJwtProvider
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}
