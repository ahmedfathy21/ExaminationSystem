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

    public LoginCommandHandler(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        IJwtProvider jwtProvider)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtProvider = jwtProvider;
    }

    public async Task<ApiResponse<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Request.Email);
        if (user == null)
            return ApiResponse<LoginResponse>.Fail("Invalid email or password");

        if (!user.EmailConfirmed)
            return ApiResponse<LoginResponse>.Fail("Please confirm your email before logging in");

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
