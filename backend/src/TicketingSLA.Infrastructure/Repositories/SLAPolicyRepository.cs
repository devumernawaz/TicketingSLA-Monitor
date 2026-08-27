using Microsoft.EntityFrameworkCore;
using TicketingSLA.Application.Interfaces;
using TicketingSLA.Domain.Entities;
using TicketingSLA.Domain.Enums;
using TicketingSLA.Infrastructure.Persistence;

namespace TicketingSLA.Infrastructure.Repositories;

public class SLAPolicyRepository : ISLAPolicyRepository
{
    private readonly ApplicationDbContext _context;

    public SLAPolicyRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SLAPolicy?> GetByIdAsync(Guid id) =>
        await _context.SLAPolicies.FirstOrDefaultAsync(p => p.Id == id);

    public async Task<SLAPolicy?> GetByPriorityAsync(TicketPriority priority) =>
        await _context.SLAPolicies.FirstOrDefaultAsync(p => p.Priority == priority);

    public async Task<IEnumerable<SLAPolicy>> GetAllAsync() =>
        await _context.SLAPolicies.ToListAsync();

    public async Task AddAsync(SLAPolicy policy) =>
        await _context.SLAPolicies.AddAsync(policy);

    public void Update(SLAPolicy policy) =>
        _context.SLAPolicies.Update(policy);

    public void Delete(SLAPolicy policy) =>
        _context.SLAPolicies.Remove(policy);

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}