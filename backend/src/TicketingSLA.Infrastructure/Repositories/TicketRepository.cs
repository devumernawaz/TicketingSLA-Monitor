using Microsoft.EntityFrameworkCore;
using TicketingSLA.Application.Interfaces;
using TicketingSLA.Domain.Entities;
using TicketingSLA.Domain.Enums;
using TicketingSLA.Infrastructure.Persistence;

namespace TicketingSLA.Infrastructure.Repositories;

public class TicketRepository : ITicketRepository
{
    private readonly ApplicationDbContext _context;

    public TicketRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Ticket?> GetByIdAsync(Guid id) =>
        await _context.Tickets.FirstOrDefaultAsync(t => t.Id == id);

    public async Task<IEnumerable<Ticket>> GetAllAsync() =>
        await _context.Tickets.ToListAsync();

    public async Task<IEnumerable<Ticket>> GetOpenTicketsAsync() =>
        await _context.Tickets
            .Where(t => t.Status != TicketStatus.Closed)
            .ToListAsync();

    public async Task<(IEnumerable<Ticket> Items, int TotalCount)> GetPagedAsync(
        TicketStatus? status, TicketPriority? priority, Guid? createdByUserId, int page, int pageSize)
    {
        var query = _context.Tickets.AsQueryable();

        if (status is not null)
            query = query.Where(t => t.Status == status);
        if (priority is not null)
            query = query.Where(t => t.Priority == priority);
        if (createdByUserId is not null)
            query = query.Where(t => t.CreatedByUserId == createdByUserId);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task AddAsync(Ticket ticket) =>
        await _context.Tickets.AddAsync(ticket);

    public void Update(Ticket ticket) =>
        _context.Tickets.Update(ticket);

    public void Delete(Ticket ticket) =>
        _context.Tickets.Remove(ticket);

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}