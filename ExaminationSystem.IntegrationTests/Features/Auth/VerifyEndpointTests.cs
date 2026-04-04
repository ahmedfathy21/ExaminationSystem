using System.Net;
using System.Net.Http.Json;
using Xunit;
using ExaminationSystem.Features.Auth.Verify;
using ExaminationSystem.Common.Wrappers;
using ExaminationSystem.Common.Data;
using ExaminationSystem.Common.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ExaminationSystem.IntegrationTests.Features.Auth;

public class VerifyEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public VerifyEndpointTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Verify_WithValidCode_ChangesUserToVerified()
    {
        // Arrange
        var testEmail = "verifytest@example.com";
        var testCode = "123456";
        Guid userId;

        // Seed with a dedicated scope so post-request assertions can use a fresh DbContext.
        using (var seedScope = _factory.Services.CreateScope())
        {
            var context = seedScope.ServiceProvider.GetRequiredService<AppDbContext>();

            var user = new User
            {
                FullName = "Verify Test",
                Email = testEmail,
                PasswordHash = "MockHash",
                IsVerified = false,
                VerificationCode = testCode,
                VerificationCodeExpiresAt = DateTime.UtcNow.AddHours(1)
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();
            userId = user.Id;
        }

        var command = new VerifyCommand(testEmail, testCode);

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/verify", command);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
        
        Assert.NotNull(result);
        Assert.True(result.Success);

        using var assertScope = _factory.Services.CreateScope();
        var assertContext = assertScope.ServiceProvider.GetRequiredService<AppDbContext>();
        var verifiedUser = await assertContext.Users.FindAsync(userId);

        Assert.True(verifiedUser!.IsVerified);
        Assert.Null(verifiedUser.VerificationCode);
    }
    
    [Fact]
    public async Task Verify_WithUnknownEmail_ReturnsNotFound()
    {
        // Arrange
        var command = new VerifyCommand("doesnotexist@example.com", "999999");

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/verify", command);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
