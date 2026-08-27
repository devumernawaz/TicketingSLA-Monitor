using TicketingSLA.Domain.Entities;
using TicketingSLA.Domain.Enums;

namespace TicketingSLA.Application.Interfaces;

public interface ISLAPolicyRepository
{
    Task<SLAPolicy?> GetByIdAsync(Guid id);
    Task<SLAPolicy?> GetByPriorityAsync(TicketPriority priority);
    Task<IEnumerable<SLAPolicy>> GetAllAsync();
    Task AddAsync(SLAPolicy policy);
    void Update(SLAPolicy policy);
    void Delete(SLAPolicy policy);
    Task SaveChangesAsync();
}