namespace ExaminationSystem.Common.Services;

public interface IOtpService
{
    string GenerateOtp();
    Task<bool> ValidateOtpAsync(string storedOtp, string enteredOtp, DateTime? expiresAt, int attempts, Action<int> updateAttempts);
}