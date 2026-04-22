using ExaminationSystem.Features.Auth.VerifyAccount.DTOs;
using FluentValidation;

namespace ExaminationSystem.Features.Auth.VerifyAccount;

public class VerifyAccountCommandValidator : AbstractValidator<VerifyAccountCommand>
{
    public VerifyAccountCommandValidator()
    {
        RuleFor(x => x.Request)
            .NotNull().WithMessage("Request is required");

        When(x => x.Request != null, () =>
        {
            RuleFor(x => x.Request!.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(255).WithMessage("Email must not exceed 255 characters");
        });
    }
}