using Microsoft.Extensions.Logging;
using TicketingSLA.Application.Interfaces;
using TicketingSLA.Domain.Entities;

namespace TicketingSLA.Infrastructure.Alerts;

public class MockAlertService : IAlertService
{
    private readonly ILogger<MockAlertService> _logger;

    public MockAlertService(ILogger<MockAlertService> logger)
    {
        _logger = logger;
    }

    public Task SendBreachAlertAsync(Ticket ticket, Guid tenantId)
    {
        _logger.LogWarning(
            "[MOCK ALERT] Tenant {TenantId}: Ticket '{Title}' (Id: {TicketId}) has breached its SLA deadline of {Deadline}.",
            tenantId, ticket.Title, ticket.Id, ticket.SlaDeadline);

        return Task.CompletedTask;
    }
}