using TicketingSLA.Domain.Entities;
using TicketingSLA.Domain.Enums;

namespace TicketingSLA.Application.Interfaces;

public interface ITicketRepository
{
    Task<Ticket?> GetByIdAsync(Guid id);
    Task<IEnumerable<Ticket>> GetAllAsync();
    Task<IEnumerable<Ticket>> GetOpenTicketsAsync();
    Task<(IEnumerable<Ticket> Items, int TotalCount)> GetPagedAsync(
        TicketStatus? status, TicketPriority? priority, Guid? createdByUserId, int page, int pageSize);
    Task AddAsync(Ticket ticket);
    void Update(Ticket ticket);
    void Delete(Ticket ticket);
    Task SaveChangesAsync();
}