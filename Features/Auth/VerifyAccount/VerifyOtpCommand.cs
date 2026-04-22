using ExaminationSystem.Common.Models;
using ExaminationSystem.Common.Services;
using ExaminationSystem.Common.Wrappers;
using ExaminationSystem.Features.Auth.VerifyAccount.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ExaminationSystem.Features.Auth.VerifyAccount;

public record VerifyOtpCommand(VerifyOtpRequest Request) : IRequest<ApiResponse<VerifyOtpResponse>>;

public class VerifyOtpCommandHandler : IRequestHandler<VerifyOtpCommand, ApiResponse<VerifyOtpResponse>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IOtpService _otpService;

    public VerifyOtpCommandHandler(UserManager<AppUser> userManager, IOtpService otpService)
    {
        _userManager = userManager;
        _otpService = otpService;
    }

    public async Task<ApiResponse<VerifyOtpResponse>> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Request.Email);
        if (user == null)
            return ApiResponse<VerifyOtpResponse>.Fail("Invalid verification code");

        if (user.EmailConfirmed)
            return ApiResponse<VerifyOtpResponse>.Fail("Email already verified");

        var isValid = await _otpService.ValidateOtpAsync(
            user.VerificationOtp ?? string.Empty,
            request.Request.Otp,
            user.VerificationOtpExpiresAt,
            user.VerificationOtpAttempts,
            newAttempt => user.VerificationOtpAttempts = newAttempt);

        if (!isValid)
        {
            user.VerificationOtpAttempts++;
            await _userManager.UpdateAsync(user);
            return ApiResponse<VerifyOtpResponse>.Fail("Invalid or expired verification code");
        }

        var result = await _userManager.ConfirmEmailAsync(user, request.Request.Otp);
        if (!result.Succeeded)
        {
            user.VerificationOtpAttempts++;
            await _userManager.UpdateAsync(user);
            return ApiResponse<VerifyOtpResponse>.Fail("Failed to verify email");
        }

        user.VerificationOtp = null;
        user.VerificationOtpExpiresAt = null;
        user.VerificationOtpAttempts = 0;
        await _userManager.UpdateAsync(user);

        return ApiResponse<VerifyOtpResponse>.Ok(new VerifyOtpResponse
        {
            Message = "Email verified successfully"
        });
    }
}