using ExaminationSystem.Common.Models;
using ExaminationSystem.Common.Services;
using ExaminationSystem.Common.Wrappers;
using ExaminationSystem.Features.Auth.ForgotPassword.DTOs;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ExaminationSystem.Features.Auth.ForgotPassword;

public record ResetPasswordCommand(ResetPasswordRequest Request) : IRequest<ApiResponse<ResetPasswordResponse>>;

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
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

            RuleFor(x => x.Request!.NewPassword)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
                .Matches("[0-9]").WithMessage("Password must contain at least one digit");
        });
    }
}

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, ApiResponse<ResetPasswordResponse>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IOtpService _otpService;

    public ResetPasswordCommandHandler(UserManager<AppUser> userManager, IOtpService otpService)
    {
        _userManager = userManager;
        _otpService = otpService;
    }

    public async Task<ApiResponse<ResetPasswordResponse>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Request.Email);
        if (user == null)
            return ApiResponse<ResetPasswordResponse>.Fail("Invalid reset code or email");

        var isValid = await _otpService.ValidateOtpAsync(
            user.PasswordResetOtp ?? string.Empty,
            request.Request.Otp,
            user.PasswordResetOtpExpiresAt,
            user.PasswordResetOtpAttempts,
            newAttempt => user.PasswordResetOtpAttempts = newAttempt);

        if (!isValid)
        {
            user.PasswordResetOtpAttempts++;
            await _userManager.UpdateAsync(user);
            return ApiResponse<ResetPasswordResponse>.Fail("Invalid reset code or email");
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, request.Request.NewPassword);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return ApiResponse<ResetPasswordResponse>.Fail($"Failed to reset password: {errors}");
        }

        user.PasswordResetOtp = null;
        user.PasswordResetOtpExpiresAt = null;
        user.PasswordResetOtpAttempts = 0;
        await _userManager.UpdateAsync(user);

        return ApiResponse<ResetPasswordResponse>.Ok(new ResetPasswordResponse
        {
            Message = "Password reset successfully"
        });
    }
}