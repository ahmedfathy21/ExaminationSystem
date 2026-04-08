namespace ExaminationSystem.Features.Auth.Login;

public class LoginResponse
{
    public string AccessToken { get; set; } = string.Empty;
}

public class LoginResult
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}
