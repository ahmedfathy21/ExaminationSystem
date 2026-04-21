using ExaminationSystem.Features.Auth.Register.DTOs;
using FluentValidation;

namespace ExaminationSystem.Features.Auth.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Request)
            .NotNull().WithMessage("Request is required");

        When(x => x.Request != null, () =>
        {
            RuleFor(x => x.Request!.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(255).WithMessage("Email must not exceed 255 characters");

            RuleFor(x => x.Request!.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
                .Matches("[0-9]").WithMessage("Password must contain at least one digit");

            RuleFor(x => x.Request!.FullName)
                .NotEmpty().WithMessage("Full name is required")
                .MaximumLength(150).WithMessage("Full name must not exceed 150 characters");

            RuleFor(x => x.Request!.Role)
                .NotEmpty().WithMessage("Role is required")
                .Must(role => role == "Admin" || role == "Student")
                .WithMessage("Role must be either 'Admin' or 'Student'");
        });
    }
}