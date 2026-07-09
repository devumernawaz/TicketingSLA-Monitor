using TicketingSLA.Domain.Enums;

namespace TicketingSLA.Domain.Entities;

public class SLAPolicy
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public TicketPriority Priority { get; private set; }
    public int ResponseTimeHours { get; private set; }

    private SLAPolicy() { }

    public SLAPolicy(string name, TicketPriority priority, int responseTimeHours)
    {
        if (responseTimeHours <= 0)
            throw new ArgumentException("Response time must be positive.", nameof(responseTimeHours));

        Id = Guid.NewGuid();
        Name = name;
        Priority = priority;
        ResponseTimeHours = responseTimeHours;
    }

    public DateTime CalculateDeadline(DateTime createdAt) => createdAt.AddHours(ResponseTimeHours);
}