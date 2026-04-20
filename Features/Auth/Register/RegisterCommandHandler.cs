using ExaminationSystem.Common.Models;
using ExaminationSystem.Common.Services;
using ExaminationSystem.Features.Auth.Register;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ExaminationSystem.Features.Auth.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResponse>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly IEmailConfirmationHelper _emailConfirmationHelper;

    public RegisterCommandHandler(
        UserManager<AppUser> userManager,
        IEmailService emailService,
        IEmailConfirmationHelper emailConfirmationHelper)
    {
        _userManager = userManager;
        _emailService = emailService;
        _emailConfirmationHelper = emailConfirmationHelper;
    }

    public async Task<RegisterResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return new RegisterResponse(
                Success: false,
                Message: "Email already registered"
            );
        }

        var user = new AppUser
        {
            Email = request.Email,
            UserName = request.Email,
            FullName = request.FullName,
            Role = Enum.Parse<UserRole>(request.Role, ignoreCase: true),
            EmailConfirmed = false
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return new RegisterResponse(
                Success: false,
                Message: $"Failed to create user: {errors}"
            );
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var confirmationLink = _emailConfirmationHelper.GenerateConfirmationLink(user.Id, token);

        await _emailService.SendEmailAsync(
            user.Email,
            "Confirm your email",
            $"<h1>Email Confirmation</h1><p>Click <a href=\"{confirmationLink}\">here</a> to confirm your email.</p>"
        );

        return new RegisterResponse(
            Success: true,
            Message: "User registered successfully. Please confirm your email.",
            UserId: user.Id
        );
    }
}

public interface IEmailConfirmationHelper
{
    string GenerateConfirmationLink(string userId, string token);
}