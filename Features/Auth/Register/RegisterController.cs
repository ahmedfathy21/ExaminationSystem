using ExaminationSystem.Common.Data;
using ExaminationSystem.Common.Exceptions;
using ExaminationSystem.Common.Models;
using ExaminationSystem.Common.Services;
using ExaminationSystem.Common.Wrappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExaminationSystem.Features.Auth.Register;

[ApiController]
[Route("api/auth")]
public class RegisterController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IEmailService _emailService;

    public RegisterController(AppDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse>> Register([FromBody] RegisterRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        {
            throw new ConflictException("User with this email already exists.");
        }

        var verificationCode = new Random().Next(100000, 999999).ToString();

        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = UserRole.Student,
            IsVerified = false,
            VerificationCode = verificationCode,
            VerificationCodeExpiresAt = DateTime.UtcNow.AddHours(24)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var emailBody = $"<p>Hi {user.FullName},</p><p>Your verification code is: <strong>{verificationCode}</strong></p>";
        
        try 
        {
            await _emailService.SendEmailAsync(user.Email, "Verify Your Account", emailBody);
        }
        catch (Exception ex)
        {
            // If email sending fails (e.g., SMTP not configured), we still log or handle gracefully
            Console.WriteLine($"Error sending email: {ex.Message}");
        }

        return Ok(ApiResponse.Ok());
    }
}
