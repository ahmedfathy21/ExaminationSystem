using ExaminationSystem.Common.Data;
using ExaminationSystem.Common.Exceptions;
using ExaminationSystem.Common.Models;
using ExaminationSystem.Common.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ExaminationSystem.Features.Auth.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly AppDbContext _context;
    private readonly IJwtProvider _jwtProvider;
    private readonly JwtSettings _jwtSettings;

    public LoginCommandHandler(AppDbContext context, IJwtProvider jwtProvider, IOptions<JwtSettings> jwtSettings)
    {
        _context = context;
        _jwtProvider = jwtProvider;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
        if (user == null)
        {
            throw new UnauthorizedException("Invalid email or password.");
        }

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedException("Invalid email or password.");
        }

        if (!user.IsVerified)
        {
            throw new ForbiddenException("Account is not verified. Please verify your account first.");
        }

        var accessToken = _jwtProvider.GenerateAccessToken(user);
        var refreshToken = _jwtProvider.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);

        await _context.SaveChangesAsync(cancellationToken);

        return new LoginResult
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }
}
