using System.Security.Cryptography;

namespace ExaminationSystem.Common.Services;

public class OtpService : IOtpService
{
    private const int OtpLength = 6;
    private const int MaxAttempts = 3;
    private readonly TimeSpan _expiry = TimeSpan.FromMinutes(10);

    public string GenerateOtp()
    {
        var randomNumber = new byte[4];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        var otp = BitConverter.ToUInt32(randomNumber, 0) % (uint)Math.Pow(10, OtpLength);
        return otp.ToString($"D{OtpLength}");
    }

    public Task<bool> ValidateOtpAsync(
        string storedOtp,
        string enteredOtp,
        DateTime? expiresAt,
        int attempts,
        Action<int> updateAttempts)
    {
        if (attempts >= MaxAttempts)
            return Task.FromResult(false);

        if (string.IsNullOrEmpty(storedOtp) || storedOtp != enteredOtp)
        {
            updateAttempts(attempts + 1);
            return Task.FromResult(false);
        }

        if (expiresAt.HasValue && expiresAt.Value < DateTime.UtcNow)
        {
            updateAttempts(attempts + 1);
            return Task.FromResult(false);
        }

        updateAttempts(0);
        return Task.FromResult(true);
    }
}