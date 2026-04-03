using FluentValidation;

namespace ExaminationSystem.Features.Auth.Register;

public class RegisterRequest
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}


public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x=>x.FullName)
            .NotEmpty().MaximumLength(150);
        RuleFor(x=>x.Email)
            .NotEmpty().EmailAddress();
        RuleFor(x=>x.Password)
            .NotEmpty().MinimumLength(6).MaximumLength(25);
    }
}