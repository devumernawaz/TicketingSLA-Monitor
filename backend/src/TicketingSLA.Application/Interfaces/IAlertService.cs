using TicketingSLA.Domain.Entities;

namespace TicketingSLA.Application.Interfaces;

public interface IAlertService
{
    Task SendBreachAlertAsync(Ticket ticket, Guid tenantId);
}