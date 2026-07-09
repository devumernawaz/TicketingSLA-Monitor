using TicketingSLA.Domain.Enums;

namespace TicketingSLA.Application.DTOs.SLAPolicies;

public class CreateSLAPolicyRequest
{
    public string Name { get; set; } = string.Empty;
    public TicketPriority Priority { get; set; }
    public int ResponseTimeHours { get; set; }
}