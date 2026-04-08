using FluentValidation;
using MediatR;

namespace ExaminationSystem.Features.Auth.ForgotPassword;

/// <summary>
/// Command to initiate a password reset flow.
/// Always returns success to prevent email enumeration attacks.
/// </summary>
public record ForgotPasswordCommand(string Email) : IRequest;

public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.");
    }
}
