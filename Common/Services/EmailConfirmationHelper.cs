namespace ExaminationSystem.Common.Services;

public interface IEmailConfirmationHelper
{
    string GenerateConfirmationLink(string userId, string token);
}

public class EmailConfirmationHelper : IEmailConfirmationHelper
{
    private readonly IConfiguration _configuration;

    public EmailConfirmationHelper(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateConfirmationLink(string userId, string token)
    {
        var baseUrl = _configuration["AppSettings:BaseUrl"] ?? "http://localhost:5000";
        var encodedToken = Uri.EscapeDataString(token);
        return $"{baseUrl}/api/auth/verify-email?userId={userId}&token={encodedToken}";
    }
}