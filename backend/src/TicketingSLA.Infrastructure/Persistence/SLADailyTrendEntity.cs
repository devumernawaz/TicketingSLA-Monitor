namespace TicketingSLA.Infrastructure.Persistence;

public class SLADailyTrendEntity
{
    public DateOnly TrendDate { get; set; }
    public int TicketsCreatedCount { get; set; }
    public int BreachedCount { get; set; }
    public double? BreachRatePercent { get; set; }
}