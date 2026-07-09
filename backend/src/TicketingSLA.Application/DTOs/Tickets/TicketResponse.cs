using TicketingSLA.Domain.Enums;

namespace TicketingSLA.Application.DTOs.Tickets;

public class TicketResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TicketStatus Status { get; set; }
    public TicketPriority Priority { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime SlaDeadline { get; set; }
    public bool IsBreached { get; set; }
    public Guid? AssignedAgentId { get; set; }
}