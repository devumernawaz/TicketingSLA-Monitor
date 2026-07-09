using TicketingSLA.Application.DTOs.Dashboard;

namespace TicketingSLA.Application.Interfaces;

public interface IDashboardRepository
{
    Task<SLADashboardSummaryResponse> GetSummaryAsync(Guid tenantId);
    Task<IEnumerable<SLADailyTrendResponse>> GetDailyTrendAsync(Guid tenantId, int daysBack = 14);
}