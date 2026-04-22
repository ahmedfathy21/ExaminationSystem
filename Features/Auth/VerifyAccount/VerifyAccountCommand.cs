using ExaminationSystem.Common.Models;
using ExaminationSystem.Common.Services;
using ExaminationSystem.Common.Wrappers;
using ExaminationSystem.Features.Auth.VerifyAccount.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ExaminationSystem.Features.Auth.VerifyAccount;

public record VerifyAccountCommand(VerifyAccountRequest Request) : IRequest<ApiResponse<VerifyAccountResponse>>;

public class VerifyAccountCommandHandler : IRequestHandler<VerifyAccountCommand, ApiResponse<VerifyAccountResponse>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IOtpService _otpService;
    private readonly IEmailService _emailService;

    public VerifyAccountCommandHandler(
        UserManager<AppUser> userManager,
        IOtpService otpService,
        IEmailService emailService)
    {
        _userManager = userManager;
        _otpService = otpService;
        _emailService = emailService;
    }

    public async Task<ApiResponse<VerifyAccountResponse>> Handle(VerifyAccountCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Request.Email);
        if (user == null)
            return ApiResponse<VerifyAccountResponse>.Fail("User not found");

        if (user.EmailConfirmed)
            return ApiResponse<VerifyAccountResponse>.Fail("Email already verified");

        var otp = _otpService.GenerateOtp();
        user.VerificationOtp = otp;
        user.VerificationOtpExpiresAt = DateTime.UtcNow.AddMinutes(10);
        user.VerificationOtpAttempts = 0;

        await _userManager.UpdateAsync(user);

        await _emailService.SendEmailAsync(
            user.Email!,
            "Verify your account",
            $"<h1>Account Verification</h1><p>Your verification code is: <strong>{otp}</strong></p><p>This code will expire in 10 minutes.</p>"
        );

        return ApiResponse<VerifyAccountResponse>.Ok(new VerifyAccountResponse
        {
            Message = "Verification code sent to your email"
        });
    }
}