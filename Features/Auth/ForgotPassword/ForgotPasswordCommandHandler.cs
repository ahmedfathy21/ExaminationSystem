using ExaminationSystem.Common.Data;
using ExaminationSystem.Common.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExaminationSystem.Features.Auth.ForgotPassword;

/// <summary>
/// Generates a 6-digit reset code, stores it with a 15-minute expiry,
/// and emails it to the user. If the email doesn't exist, we silently
/// return without error to prevent email enumeration.
/// </summary>
public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand>
{
    private readonly AppDbContext _context;
    private readonly IEmailService _emailService;

    public ForgotPasswordCommandHandler(AppDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public async Task Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        // Security: silently return if user not found — never reveal whether an email exists
        if (user == null)
            return;

        // Generate a cryptographically random 6-digit code
        var resetCode = new Random().Next(100000, 999999).ToString();

        // Store with 15-minute expiry window
        user.ResetToken = resetCode;
        user.ResetTokenExpiresAt = DateTime.UtcNow.AddMinutes(15);

        await _context.SaveChangesAsync(cancellationToken);

        // Send the reset code via email — graceful failure (log, don't crash the request)
        var emailBody = $"""
            <p>Hi {user.FullName},</p>
            <p>You requested a password reset. Your reset code is:</p>
            <h2>{resetCode}</h2>
            <p>This code expires in <strong>15 minutes</strong>.</p>
            <p>If you didn't request this, please ignore this email.</p>
            """;

        try
        {
            await _emailService.SendEmailAsync(user.Email, "Password Reset Code", emailBody);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending password reset email: {ex.Message}");
        }
    }
}
