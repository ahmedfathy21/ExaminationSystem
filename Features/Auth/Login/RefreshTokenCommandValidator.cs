using ExaminationSystem.Features.Auth.Login.DTOs;
using FluentValidation;

namespace ExaminationSystem.Features.Auth.Login;

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.Request)
            .NotNull().WithMessage("Request is required");

        When(x => x.Request != null, () =>
        {
            RuleFor(x => x.Request!.RefreshToken)
                .NotEmpty().WithMessage("Refresh token is required");
        });
    }
}