using Microsoft.EntityFrameworkCore;
using TicketingSLA.Domain.Entities;
using TicketingSLA.Domain.Enums;
using TicketingSLA.Infrastructure.Persistence;
using TicketingSLA.IntegrationTests.Fixtures;
using Xunit;

namespace TicketingSLA.IntegrationTests.MultiTenancy;

public class TenantIsolationTests : IClassFixture<SqlServerTestFixture>
{
    private readonly SqlServerTestFixture _fixture;

    public TenantIsolationTests(SqlServerTestFixture fixture)
    {
        _fixture = fixture;
    }

    private ApplicationDbContext CreateContext(Guid tenantId)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlServer(_fixture.ConnectionString);
        return new ApplicationDbContext(optionsBuilder.Options, new FixedTenantService(tenantId));
    }

    [Fact]
    public async Task Ticket_CreatedByOneTenant_IsInvisibleToAnotherTenant()
    {
        var tenantAId = Guid.NewGuid();
        var tenantBId = Guid.NewGuid();

        // Arrange: Tenant A creates an SLA policy and a ticket
        await using (var contextA = CreateContext(tenantAId))
        {
            var policy = new SLAPolicy("Standard", TicketPriority.Medium, 4);
            contextA.SLAPolicies.Add(policy);
            await contextA.SaveChangesAsync();

            var ticket = new Ticket("Tenant A's private ticket", "Should not leak to Tenant B", policy);
            contextA.Tickets.Add(ticket);
            await contextA.SaveChangesAsync();
        }

        // Act: Tenant B queries all tickets
        await using var contextB = CreateContext(tenantBId);
        var tenantBTickets = await contextB.Tickets.ToListAsync();

        // Assert: Tenant B sees nothing, even though the row genuinely exists in the database
        Assert.Empty(tenantBTickets);
    }

    [Fact]
    public async Task Ticket_CreatedByOneTenant_IsVisibleToThatSameTenant()
    {
        var tenantId = Guid.NewGuid();

        await using var context = CreateContext(tenantId);

        var policy = new SLAPolicy("Standard", TicketPriority.Medium, 4);
        context.SLAPolicies.Add(policy);
        await context.SaveChangesAsync();

        var ticket = new Ticket("Visible to owner", "Should be visible", policy);
        context.Tickets.Add(ticket);
        await context.SaveChangesAsync();

        var tickets = await context.Tickets.ToListAsync();

        Assert.Single(tickets);
        Assert.Equal("Visible to owner", tickets.First().Title);
    }
}