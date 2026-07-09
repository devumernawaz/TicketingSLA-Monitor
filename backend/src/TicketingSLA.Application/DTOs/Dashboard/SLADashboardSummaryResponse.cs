namespace TicketingSLA.Application.DTOs.Dashboard;

public class SLADashboardSummaryResponse
{
    public int OpenTicketCount { get; set; }
    public int AtRiskCount { get; set; }
    public int BreachedCount { get; set; }
    public double? AvgResponseTimeMinutes { get; set; }
    public double? BreachRateLast24HoursPercent { get; set; }
}