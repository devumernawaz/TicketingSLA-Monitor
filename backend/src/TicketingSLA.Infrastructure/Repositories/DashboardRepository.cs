using Microsoft.EntityFrameworkCore;
using TicketingSLA.Application.DTOs.Dashboard;
using TicketingSLA.Application.Interfaces;
using TicketingSLA.Infrastructure.Persistence;

namespace TicketingSLA.Infrastructure.Repositories;

public class DashboardRepository : IDashboardRepository
{
    private readonly ApplicationDbContext _context;

    public DashboardRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<SLADailyTrendResponse>> GetDailyTrendAsync(Guid tenantId, int daysBack = 14)
    {
        var results = await _context.SLADailyTrends
            .FromSqlInterpolated($"EXEC dbo.GetSLADailyTrend @TenantId = {tenantId}, @DaysBack = {daysBack}")
            .ToListAsync();

        return results.Select(r => new SLADailyTrendResponse
        {
            TrendDate = r.TrendDate,
            TicketsCreatedCount = r.TicketsCreatedCount,
            BreachedCount = r.BreachedCount,
            BreachRatePercent = r.BreachRatePercent
        });
    }

    public async Task<SLADashboardSummaryResponse> GetSummaryAsync(Guid tenantId)
    {
        var result = await _context.SLADashboardSummaries
            .FromSqlInterpolated($"EXEC dbo.GetSLADashboardSummary @TenantId = {tenantId}")
            .ToListAsync();

        var summary = result.FirstOrDefault() ?? new SLADashboardSummaryEntity();

        return new SLADashboardSummaryResponse
        {
            OpenTicketCount = summary.OpenTicketCount,
            AtRiskCount = summary.AtRiskCount,
            BreachedCount = summary.BreachedCount,
            AvgResponseTimeMinutes = summary.AvgResponseTimeMinutes,
            BreachRateLast24HoursPercent = summary.BreachRateLast24HoursPercent
        };
    }
}