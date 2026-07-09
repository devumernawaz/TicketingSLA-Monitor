using Microsoft.EntityFrameworkCore;
using TicketingSLA.Application.Interfaces;
using TicketingSLA.Domain.Entities;
using TicketingSLA.Infrastructure.Persistence;

namespace TicketingSLA.Infrastructure.Repositories;

public class TenantRepository : ITenantRepository
{
    private readonly ApplicationDbContext _context;

    public TenantRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Tenant?> GetByIdAsync(Guid id) =>
        await _context.Tenants.FirstOrDefaultAsync(t => t.Id == id);

    public async Task<IEnumerable<Tenant>> GetAllAsync() =>
        await _context.Tenants.ToListAsync();

    public async Task AddAsync(Tenant tenant) =>
        await _context.Tenants.AddAsync(tenant);

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}