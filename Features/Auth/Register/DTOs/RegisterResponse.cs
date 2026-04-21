namespace ExaminationSystem.Features.Auth.Register.DTOs;

public class RegisterResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public DateTime? ExpiresAt { get; set; }

    public static RegisterResponse SuccessResponse(string userId, string message = "User registered successfully. Please confirm your email.")
    {
        return new RegisterResponse
        {
            Success = true,
            Message = message,
            UserId = userId
        };
    }

    public static RegisterResponse Failure(string message)
    {
        return new RegisterResponse
        {
            Success = false,
            Message = message
        };
    }
}