using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using TicketingSLA.Application.DTOs.Auth;
using TicketingSLA.Application.DTOs.Tenants;
using TicketingSLA.Domain.Enums;
using TicketingSLA.IntegrationTests.Fixtures;
using Xunit;

namespace TicketingSLA.IntegrationTests.Authorization;

public class EndpointAuthorizationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public EndpointAuthorizationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private async Task<Guid> CreateTenantAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/api/tenants", new CreateTenantRequest { Name = $"Tenant-{Guid.NewGuid()}" });
        var tenant = await response.Content.ReadFromJsonAsync<TenantResponse>();
        return tenant!.Id;
    }

    private async Task<HttpClient> CreateAuthenticatedClientAsync(UserRole role, Guid? tenantId = null)
    {
        var client = _factory.CreateClient();
        var resolvedTenantId = tenantId ?? await CreateTenantAsync(client);

        var registerResponse = await client.PostAsJsonAsync("/api/auth/register", new RegisterRequest
        {
            Email = $"user-{Guid.NewGuid():N}@test.local",
            Password = "Passw0rd!",
            DisplayName = "Test User",
            TenantId = resolvedTenantId,
            Role = role,
        });
        var auth = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth!.Token);
        return client;
    }

    [Fact]
    public async Task GetTickets_ReturnsUnauthorized_WithoutToken()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/tickets");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetDashboardSummary_ReturnsForbidden_ForClientRole()
    {
        var client = await CreateAuthenticatedClientAsync(UserRole.Client);

        var response = await client.GetAsync("/api/dashboard/summary");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetDashboardSummary_ReturnsOk_ForAdminRole()
    {
        var client = await CreateAuthenticatedClientAsync(UserRole.Admin);

        var response = await client.GetAsync("/api/dashboard/summary");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateSLAPolicy_ReturnsForbidden_ForAgentRole()
    {
        var client = await CreateAuthenticatedClientAsync(UserRole.Agent);

        var response = await client.PostAsJsonAsync("/api/slapolicies", new
        {
            name = "Standard",
            priority = "Medium",
            responseTimeHours = 4,
        });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task DeleteTicket_ReturnsForbidden_ForClientRole()
    {
        var client = await CreateAuthenticatedClientAsync(UserRole.Client);

        var response = await client.DeleteAsync($"/api/tickets/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Client_CanCreateTicket_ButCannotAssignOrCloseIt()
    {
        var setupClient = _factory.CreateClient();
        var tenantId = await CreateTenantAsync(setupClient);

        var admin = await CreateAuthenticatedClientAsync(UserRole.Admin, tenantId);
        var policyResponse = await admin.PostAsJsonAsync("/api/slapolicies", new
        {
            name = "Standard",
            priority = "Medium",
            responseTimeHours = 4,
        });
        Assert.Equal(HttpStatusCode.OK, policyResponse.StatusCode);

        var client = await CreateAuthenticatedClientAsync(UserRole.Client, tenantId);

        var createResponse = await client.PostAsJsonAsync("/api/tickets", new
        {
            title = "Cannot log in",
            description = "Getting a 500 error",
            priority = "Medium",
        });
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var ticketId = (await createResponse.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("id").GetGuid();

        var assignResponse = await client.PatchAsJsonAsync($"/api/tickets/{ticketId}/assign", new { agentId = Guid.NewGuid() });
        Assert.Equal(HttpStatusCode.Forbidden, assignResponse.StatusCode);

        var closeResponse = await client.PatchAsync($"/api/tickets/{ticketId}/close", content: null);
        Assert.Equal(HttpStatusCode.Forbidden, closeResponse.StatusCode);
    }
}
