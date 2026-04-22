using ExaminationSystem.Common.Models;
using ExaminationSystem.Common.Services;
using ExaminationSystem.Common.Wrappers;
using ExaminationSystem.Features.Auth.Login.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ExaminationSystem.Features.Auth.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, ApiResponse<LoginResponse>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IJwtProvider _jwtProvider;
    private readonly IOtpService _otpService;
    private readonly IEmailService _emailService;

    public LoginCommandHandler(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        IJwtProvider jwtProvider,
        IOtpService otpService,
        IEmailService emailService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtProvider = jwtProvider;
        _otpService = otpService;
        _emailService = emailService;
    }

    public async Task<ApiResponse<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Request.Email);
        if (user == null)
            return ApiResponse<LoginResponse>.Fail("Invalid email or password");

        if (!user.EmailConfirmed)
        {
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

            return ApiResponse<LoginResponse>.Fail("Please verify your email. A verification code has been sent.");
        }

        var result = await _signInManager.PasswordSignInAsync(
            user,
            request.Request.Password,
            isPersistent: false,
            lockoutOnFailure: true);

        if (result.IsLockedOut)
            return ApiResponse<LoginResponse>.Fail("Account is locked. Please try again later");

        if (!result.Succeeded)
        {
            var accessFailedCount = await _userManager.GetAccessFailedCountAsync(user);
            var remainingAttempts = 3 - accessFailedCount;
            return ApiResponse<LoginResponse>.Fail($"Invalid email or password. {remainingAttempts} attempts remaining");
        }

        await _userManager.ResetAccessFailedCountAsync(user);

        var accessToken = _jwtProvider.GenerateAccessToken(user);
        var refreshToken = _jwtProvider.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);
        await _userManager.UpdateAsync(user);

        return ApiResponse<LoginResponse>.Ok(new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName,
            Role = user.Role.ToString()
        });
    }
}
