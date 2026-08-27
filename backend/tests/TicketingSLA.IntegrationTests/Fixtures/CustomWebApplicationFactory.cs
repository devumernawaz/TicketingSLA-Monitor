using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace TicketingSLA.IntegrationTests.Fixtures;

// Boots the real API pipeline (auth, authorization, middleware) against the
// Testcontainers SQL Server instance, instead of constructing ApplicationDbContext directly.
public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly SqlServerTestFixture _dbFixture = new();

    public Task InitializeAsync() => _dbFixture.InitializeAsync();

    public new async Task DisposeAsync()
    {
        await _dbFixture.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = _dbFixture.ConnectionString,
                ["Jwt:Key"] = "integration-test-only-signing-key-0123456789-not-for-production",
                ["Jwt:Issuer"] = "TicketingSLA.API.Tests",
                ["Jwt:Audience"] = "TicketingSLA.Client.Tests",
                ["Jwt:ExpiryMinutes"] = "60",
            });
        });
    }
}
