using TicketingSLA.Domain.Enums;

namespace TicketingSLA.Application.DTOs.SLAPolicies;

public class SLAPolicyResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public TicketPriority Priority { get; set; }
    public int ResponseTimeHours { get; set; }
}