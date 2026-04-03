using FluentValidation;
using MediatR;

namespace ExaminationSystem.Features.Auth.Verify;

public record VerifyCommand(string Email, string Code) : IRequest;

public class VerifyCommandValidator : AbstractValidator<VerifyCommand>
{
    public VerifyCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
            
        RuleFor(x => x.Code)
            .NotEmpty()
            .Length(6);
    }
}
