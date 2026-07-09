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

    public async Task AddAsync(Ticket ticket) =>
        await _context.Tickets.AddAsync(ticket);

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}