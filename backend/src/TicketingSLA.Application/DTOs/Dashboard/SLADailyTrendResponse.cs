namespace TicketingSLA.Application.DTOs.Dashboard;

public class SLADailyTrendResponse
{
    public DateOnly TrendDate { get; set; }
    public int TicketsCreatedCount { get; set; }
    public int BreachedCount { get; set; }
    public double? BreachRatePercent { get; set; }
}