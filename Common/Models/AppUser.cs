using Microsoft.AspNetCore.Identity;

public class AppUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Student;
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiresAt { get; set; }
    public string? VerificationOtp { get; set; }
    public DateTime? VerificationOtpExpiresAt { get; set; }
    public int VerificationOtpAttempts { get; set; }
    public string? PasswordResetOtp { get; set; }
    public DateTime? PasswordResetOtpExpiresAt { get; set; }
    public int PasswordResetOtpAttempts { get; set; }
}

public enum UserRole
{
    Admin,
    Student,
}