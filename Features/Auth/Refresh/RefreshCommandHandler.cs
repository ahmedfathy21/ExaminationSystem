using ExaminationSystem.Common.Data;
using ExaminationSystem.Common.Exceptions;
using ExaminationSystem.Common.Models;
using ExaminationSystem.Common.Services;
using ExaminationSystem.Features.Auth.Login;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ExaminationSystem.Features.Auth.Refresh;

public class RefreshCommandHandler : IRequestHandler<RefreshCommand, LoginResult>
{
    private readonly AppDbContext _context;
    private readonly IJwtProvider _jwtProvider;
    private readonly JwtSettings _jwtSettings;

    public RefreshCommandHandler(AppDbContext context, IJwtProvider jwtProvider, IOptions<JwtSettings> jwtSettings)
    {
        _context = context;
        _jwtProvider = jwtProvider;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<LoginResult> Handle(RefreshCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken, cancellationToken);

        if (user == null || user.RefreshTokenExpiresAt < DateTime.UtcNow)
        {
            throw new UnauthorizedException("Invalid or expired refresh token. Please login again.");
        }

        var accessToken = _jwtProvider.GenerateAccessToken(user);
        var newRefreshToken = _jwtProvider.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);

        await _context.SaveChangesAsync(cancellationToken);

        return new LoginResult
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken
        };
    }
}
