using TicketingSLA.Domain.Entities;

namespace TicketingSLA.Application.Interfaces;

public interface ITicketRepository
{
    Task<Ticket?> GetByIdAsync(Guid id);
    Task<IEnumerable<Ticket>> GetAllAsync();
    Task<IEnumerable<Ticket>> GetOpenTicketsAsync();
    Task AddAsync(Ticket ticket);
    Task SaveChangesAsync();
}