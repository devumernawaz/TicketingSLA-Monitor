using TicketingSLA.Application.DTOs.Dashboard;
using TicketingSLA.Application.Interfaces;

namespace TicketingSLA.Application.Services;

public class DashboardService
{
    private readonly IDashboardRepository _dashboardRepository;

    public DashboardService(IDashboardRepository dashboardRepository)
    {
        _dashboardRepository = dashboardRepository;
    }

    public async Task<SLADashboardSummaryResponse> GetSummaryAsync(Guid tenantId)
    {
        return await _dashboardRepository.GetSummaryAsync(tenantId);
    }
    public async Task<IEnumerable<SLADailyTrendResponse>> GetDailyTrendAsync(Guid tenantId, int daysBack = 14)
    {
        return await _dashboardRepository.GetDailyTrendAsync(tenantId, daysBack);
    }
}