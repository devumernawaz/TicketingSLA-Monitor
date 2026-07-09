using TicketingSLA.Application.Interfaces;

namespace TicketingSLA.IntegrationTests.Fixtures;

public class FixedTenantService : ICurrentTenantService
{
    public FixedTenantService(Guid tenantId) => TenantId = tenantId;
    public Guid TenantId { get; }
}