using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;
using TicketingSLA.Infrastructure.Persistence;
using Xunit;

namespace TicketingSLA.IntegrationTests.Fixtures;

public class SqlServerTestFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _container = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .Build();

    public string ConnectionString => _container.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlServer(ConnectionString);

        // A throwaway ICurrentTenantService just to satisfy the constructor —
        // migrations don't query tenant-filtered data, so the value is irrelevant here.
        await using var context = new ApplicationDbContext(optionsBuilder.Options, new NullCurrentTenantService());
        await context.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}