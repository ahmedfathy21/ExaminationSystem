using ExaminationSystem.Common.Models;
using ExaminationSystem.Common.Services;
using ExaminationSystem.Common.Wrappers;
using ExaminationSystem.Features.Auth.ForgotPassword.DTOs;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ExaminationSystem.Features.Auth.ForgotPassword;

public record ForgotPasswordCommand(ForgotPasswordRequest Request) : IRequest<ApiResponse<ForgotPasswordResponse>>;

public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
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

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, ApiResponse<ForgotPasswordResponse>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IOtpService _otpService;
    private readonly IEmailService _emailService;

    public ForgotPasswordCommandHandler(
        UserManager<AppUser> userManager,
        IOtpService otpService,
        IEmailService emailService)
    {
        _userManager = userManager;
        _otpService = otpService;
        _emailService = emailService;
    }

    public async Task<ApiResponse<ForgotPasswordResponse>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Request.Email);
        if (user == null)
            return ApiResponse<ForgotPasswordResponse>.Ok(new ForgotPasswordResponse
            {
                Message = "If the email exists, a reset code will be sent"
            });

        var otp = _otpService.GenerateOtp();
        user.PasswordResetOtp = otp;
        user.PasswordResetOtpExpiresAt = DateTime.UtcNow.AddMinutes(10);
        user.PasswordResetOtpAttempts = 0;

        await _userManager.UpdateAsync(user);

        await _emailService.SendEmailAsync(
            user.Email!,
            "Password Reset",
            $"<h1>Password Reset</h1><p>Your reset code is: <strong>{otp}</strong></p><p>This code will expire in 10 minutes.</p>"
        );

        return ApiResponse<ForgotPasswordResponse>.Ok(new ForgotPasswordResponse
        {
            Message = "If the email exists, a reset code will be sent"
        });
    }
}