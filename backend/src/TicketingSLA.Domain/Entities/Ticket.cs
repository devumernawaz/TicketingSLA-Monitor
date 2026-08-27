using TicketingSLA.Domain.Enums;

namespace TicketingSLA.Domain.Entities;

public class Ticket
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public TicketStatus Status { get; private set; }
    public TicketPriority Priority { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime SlaDeadline { get; private set; }
    public DateTime? BreachedAt { get; private set; }
    public Guid? AssignedAgentId { get; private set; }
    public Guid? CreatedByUserId { get; private set; }

    private Ticket() { }

    public Ticket(string title, string description, SLAPolicy slaPolicy, Guid? createdByUserId = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty.", nameof(title));

        Id = Guid.NewGuid();
        Title = title;
        Description = description;
        Priority = slaPolicy.Priority;
        Status = TicketStatus.Open;
        CreatedAt = DateTime.UtcNow;
        SlaDeadline = slaPolicy.CalculateDeadline(CreatedAt);
        CreatedByUserId = createdByUserId;
    }

    public bool IsBreached(DateTime asOf) => Status != TicketStatus.Closed && asOf > SlaDeadline;

    public bool IsAtRiskOfBreach(DateTime asOf, TimeSpan warningWindow) =>
        Status != TicketStatus.Closed && !IsBreached(asOf) && (SlaDeadline - asOf) <= warningWindow;

    public void MarkBreached(DateTime detectedAt)
    {
        if (BreachedAt is null && IsBreached(detectedAt))
            BreachedAt = detectedAt;
    }

    public void AssignTo(Guid agentId)
    {
        if (Status == TicketStatus.Closed)
            throw new InvalidOperationException("Cannot assign a closed ticket.");

        AssignedAgentId = agentId;
        Status = TicketStatus.InProgress;
    }

    public void Close()
    {
        Status = TicketStatus.Closed;
    }
}