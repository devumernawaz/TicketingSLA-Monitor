using System.Net;
using System.Net.Http.Json;
using TicketingSLA.Application.DTOs.Auth;
using TicketingSLA.Application.DTOs.Tenants;
using TicketingSLA.Domain.Enums;
using TicketingSLA.IntegrationTests.Fixtures;
using Xunit;

namespace TicketingSLA.IntegrationTests.Auth;

public class AuthFlowTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthFlowTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<TenantResponse> CreateTenantAsync()
    {
        var response = await _client.PostAsJsonAsync("/api/tenants", new CreateTenantRequest { Name = $"Tenant-{Guid.NewGuid()}" });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<TenantResponse>())!;
    }

    [Fact]
    public async Task Register_ThenLogin_ReturnsValidToken()
    {
        var tenant = await CreateTenantAsync();

        var registerRequest = new RegisterRequest
        {
            Email = $"user-{Guid.NewGuid():N}@test.local",
            Password = "Passw0rd!",
            DisplayName = "Test User",
            TenantId = tenant.Id,
            Role = UserRole.Admin,
        };

        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);
        var registerBody = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.False(string.IsNullOrWhiteSpace(registerBody!.Token));
        Assert.Equal(UserRole.Admin, registerBody.Role);

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            Email = registerRequest.Email,
            Password = registerRequest.Password,
        });

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        var loginBody = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.False(string.IsNullOrWhiteSpace(loginBody!.Token));
        Assert.Equal(registerRequest.Email, loginBody.Email);
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_ForWrongPassword()
    {
        var tenant = await CreateTenantAsync();
        var email = $"user-{Guid.NewGuid():N}@test.local";

        await _client.PostAsJsonAsync("/api/auth/register", new RegisterRequest
        {
            Email = email,
            Password = "Passw0rd!",
            DisplayName = "Test User",
            TenantId = tenant.Id,
            Role = UserRole.Client,
        });

        var response = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest { Email = email, Password = "WrongPassword1!" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Register_ReturnsBadRequest_ForDuplicateEmail()
    {
        var tenant = await CreateTenantAsync();

        var request = new RegisterRequest
        {
            Email = $"user-{Guid.NewGuid():N}@test.local",
            Password = "Passw0rd!",
            DisplayName = "Test User",
            TenantId = tenant.Id,
            Role = UserRole.Client,
        };

        var first = await _client.PostAsJsonAsync("/api/auth/register", request);
        Assert.Equal(HttpStatusCode.OK, first.StatusCode);

        var second = await _client.PostAsJsonAsync("/api/auth/register", request);
        Assert.Equal(HttpStatusCode.BadRequest, second.StatusCode);
    }

    [Fact]
    public async Task Register_ReturnsBadRequest_ForUnknownTenant()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/register", new RegisterRequest
        {
            Email = $"user-{Guid.NewGuid():N}@test.local",
            Password = "Passw0rd!",
            DisplayName = "Test User",
            TenantId = Guid.NewGuid(),
            Role = UserRole.Client,
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
