using System.Net;
using System.Net.Http.Json;
using Xunit;
using ExaminationSystem.Features.Auth.Register;
using ExaminationSystem.Common.Wrappers;

namespace ExaminationSystem.IntegrationTests.Features.Auth;

public class RegisterEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public RegisterEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_WithValidData_ReturnsOkResult()
    {
        // Arrange
        var request = new RegisterRequest
        {
            FullName = "John Doe",
            Email = "johndoe@example.com",
            Password = "SecurePassword123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
        Assert.NotNull(result);
        Assert.True(result.Success);
    }
    
    [Fact]
    public async Task Register_WithDuplicateEmail_ReturnsConflict()
    {
        // Arrange
        var request = new RegisterRequest
        {
            FullName = "Jane Doe",
            Email = "janedoe@example.com",
            Password = "SecurePassword123!"
        };

        await _client.PostAsJsonAsync("/api/auth/register", request);

        // Act - Attempt to register again
        var duplicateResponse = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, duplicateResponse.StatusCode);
    }
}
