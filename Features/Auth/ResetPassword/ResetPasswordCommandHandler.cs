using ExaminationSystem.Common.Data;
using ExaminationSystem.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExaminationSystem.Features.Auth.ResetPassword;

/// <summary>
/// Validates the reset code, hashes the new password, clears the reset token,
/// and revokes the refresh token to force re-login on all devices.
/// </summary>
public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand>
{
    private readonly AppDbContext _context;

    public ResetPasswordCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user == null)
        {
            throw new NotFoundException("No account found with this email.");
        }

        // Validate the reset code exists and matches
        if (string.IsNullOrEmpty(user.ResetToken) || user.ResetToken != request.Code)
        {
            throw new UnauthorizedException("Invalid reset code.");
        }

        // Validate the code hasn't expired (15-minute window)
        if (user.ResetTokenExpiresAt < DateTime.UtcNow)
        {
            // Clear the expired token so it can't be retried
            user.ResetToken = null;
            user.ResetTokenExpiresAt = null;
            await _context.SaveChangesAsync(cancellationToken);

            throw new UnauthorizedException("Reset code has expired. Please request a new one.");
        }

        // ── Apply password reset ──────────────────────────────────
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

        // Clear reset token (one-time use)
        user.ResetToken = null;
        user.ResetTokenExpiresAt = null;

        // Revoke refresh token — forces re-login on all devices
        user.RefreshToken = null;
        user.RefreshTokenExpiresAt = null;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
