using ExaminationSystem.Common.Models;

namespace ExaminationSystem.Common.Services;

public interface IJwtProvider
{
    string GenerateAccessToken(AppUser user);
    string GenerateRefreshToken();
}
