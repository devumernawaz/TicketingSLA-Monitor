namespace TicketingSLA.Application.DTOs.SLAPolicies;

public class UpdateSLAPolicyRequest
{
    public string Name { get; set; } = string.Empty;
    public int ResponseTimeHours { get; set; }
}
