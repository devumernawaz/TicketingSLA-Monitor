using TicketingSLA.Domain.Entities;
using TicketingSLA.Domain.Enums;
using Xunit;

namespace TicketingSLA.UnitTests.Domain;

public class TicketTests
{
    private static SLAPolicy CreatePolicy(int responseTimeHours = 4) =>
        new("Standard Response", TicketPriority.Medium, responseTimeHours);

    [Fact]
    public void IsBreached_ReturnsFalse_WhenWithinDeadline()
    {
        // Arrange
        var policy = CreatePolicy(responseTimeHours: 4);
        var ticket = new Ticket("Server down", "Prod outage", policy);

        // Act — check 1 hour after creation, well within the 4-hour SLA
        var result = ticket.IsBreached(ticket.CreatedAt.AddHours(1));

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsBreached_ReturnsTrue_WhenPastDeadline()
    {
        // Arrange
        var policy = CreatePolicy(responseTimeHours: 4);
        var ticket = new Ticket("Server down", "Prod outage", policy);

        // Act — check 5 hours after creation, past the 4-hour SLA
        var result = ticket.IsBreached(ticket.CreatedAt.AddHours(5));

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsBreached_ReturnsFalse_WhenTicketIsClosed_EvenIfPastDeadline()
    {
        // Arrange
        var policy = CreatePolicy(responseTimeHours: 4);
        var ticket = new Ticket("Server down", "Prod outage", policy);
        ticket.Close();

        // Act — well past deadline, but closed tickets can't be "breached"
        var result = ticket.IsBreached(ticket.CreatedAt.AddHours(10));

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsAtRiskOfBreach_ReturnsTrue_WhenWithinWarningWindow()
    {
        // Arrange
        var policy = CreatePolicy(responseTimeHours: 4);
        var ticket = new Ticket("Server down", "Prod outage", policy);
        var warningWindow = TimeSpan.FromHours(1);

        // Act — check at 3h30m, which is within 1 hour of the 4-hour deadline
        var result = ticket.IsAtRiskOfBreach(ticket.CreatedAt.AddHours(3.5), warningWindow);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsAtRiskOfBreach_ReturnsFalse_WhenFarFromDeadline()
    {
        // Arrange
        var policy = CreatePolicy(responseTimeHours: 4);
        var ticket = new Ticket("Server down", "Prod outage", policy);
        var warningWindow = TimeSpan.FromHours(1);

        // Act — check at 1h, nowhere near the 4-hour deadline
        var result = ticket.IsAtRiskOfBreach(ticket.CreatedAt.AddHours(1), warningWindow);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void MarkBreached_SetsBreachedAt_OnlyOnce()
    {
        // Arrange
        var policy = CreatePolicy(responseTimeHours: 4);
        var ticket = new Ticket("Server down", "Prod outage", policy);
        var firstDetection = ticket.CreatedAt.AddHours(5);
        var secondDetection = ticket.CreatedAt.AddHours(6);

        // Act
        ticket.MarkBreached(firstDetection);
        ticket.MarkBreached(secondDetection); // should be a no-op, already marked

        // Assert
        Assert.Equal(firstDetection, ticket.BreachedAt);
    }
}