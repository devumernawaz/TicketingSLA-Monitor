using Moq;
using TicketingSLA.Application.DTOs.Dashboard;
using TicketingSLA.Application.Interfaces;
using TicketingSLA.Application.Services;
using Xunit;

namespace TicketingSLA.UnitTests.Application;

public class DashboardServiceTests
{
    [Fact]
    public async Task GetSummaryAsync_ReturnsRepositoryResult()
    {
        var tenantId = Guid.NewGuid();
        var summary = new SLADashboardSummaryResponse { OpenTicketCount = 3 };
        var repo = new Mock<IDashboardRepository>();
        repo.Setup(r => r.GetSummaryAsync(tenantId)).ReturnsAsync(summary);

        var service = new DashboardService(repo.Object);

        var result = await service.GetSummaryAsync(tenantId);

        Assert.Same(summary, result);
    }

    [Fact]
    public async Task GetDailyTrendAsync_PassesDaysBackThrough()
    {
        var tenantId = Guid.NewGuid();
        var trend = new List<SLADailyTrendResponse>();
        var repo = new Mock<IDashboardRepository>();
        repo.Setup(r => r.GetDailyTrendAsync(tenantId, 30)).ReturnsAsync(trend);

        var service = new DashboardService(repo.Object);

        await service.GetDailyTrendAsync(tenantId, 30);

        repo.Verify(r => r.GetDailyTrendAsync(tenantId, 30), Times.Once);
    }
}
