using ExaminationSystem.Features.Auth.VerifyAccount.DTOs;
using FluentValidation;

namespace ExaminationSystem.Features.Auth.VerifyAccount;

public class VerifyOtpCommandValidator : AbstractValidator<VerifyOtpCommand>
{
    public VerifyOtpCommandValidator()
    {
        RuleFor(x => x.Request)
            .NotNull().WithMessage("Request is required");

        When(x => x.Request != null, () =>
        {
            RuleFor(x => x.Request!.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(255).WithMessage("Email must not exceed 255 characters");

            RuleFor(x => x.Request!.Otp)
                .NotEmpty().WithMessage("OTP is required")
                .Length(6).WithMessage("OTP must be 6 digits")
                .Matches("^[0-9]+$").WithMessage("OTP must contain only numbers");
        });
    }
}