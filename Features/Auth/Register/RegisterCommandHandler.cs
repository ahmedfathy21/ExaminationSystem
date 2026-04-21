using ExaminationSystem.Common.Models;
using ExaminationSystem.Common.Services;
using ExaminationSystem.Features.Auth.Register;
using ExaminationSystem.Features.Auth.Register.DTOs;
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
        var existingUser = await _userManager.FindByEmailAsync(request.Request.Email);
        if (existingUser != null)
        {
            return RegisterResponse.Failure("Email already registered");
        }

        var user = new AppUser
        {
            Email = request.Request.Email,
            UserName = request.Request.Email,
            FullName = request.Request.FullName,
            Role = Enum.Parse<UserRole>(request.Request.Role, ignoreCase: true),
            EmailConfirmed = false
        };

        var result = await _userManager.CreateAsync(user, request.Request.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return RegisterResponse.Failure($"Failed to create user: {errors}");
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var confirmationLink = _emailConfirmationHelper.GenerateConfirmationLink(user.Id, token);

        await _emailService.SendEmailAsync(
            user.Email,
            "Confirm your email",
            $"<h1>Email Confirmation</h1><p>Click <a href=\"{confirmationLink}\">here</a> to confirm your email.</p>"
        );

        return RegisterResponse.SuccessResponse(user.Id);
    }
}