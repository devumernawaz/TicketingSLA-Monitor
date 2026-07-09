using TicketingSLA.Application.Interfaces;

namespace TicketingSLA.IntegrationTests.Fixtures;

public class NullCurrentTenantService : ICurrentTenantService
{
    public Guid TenantId => Guid.Empty;
}