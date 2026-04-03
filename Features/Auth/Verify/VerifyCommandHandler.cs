using ExaminationSystem.Common.Data;
using ExaminationSystem.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExaminationSystem.Features.Auth.Verify;

public class VerifyCommandHandler : IRequestHandler<VerifyCommand>
{
    private readonly AppDbContext _context;

    public VerifyCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(VerifyCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user == null)
        {
            throw new NotFoundException("User");
        }

        if (user.IsVerified)
        {
            throw new ConflictException("User is already verified.");
        }

        if (user.VerificationCode != request.Code)
        {
            throw new UnauthorizedException("Invalid verification code.");
        }

        if (user.VerificationCodeExpiresAt < DateTime.UtcNow)
        {
            throw new UnauthorizedException("Verification code has expired.");
        }

        // Apply the domain state changes
        user.IsVerified = true;
        user.VerificationCode = null;
        user.VerificationCodeExpiresAt = null;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
