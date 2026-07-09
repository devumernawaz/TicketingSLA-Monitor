using TicketingSLA.Application.Interfaces;

namespace TicketingSLA.Infrastructure.Persistence.DesignTime;

// Used ONLY by EF Core tooling (Add-Migration, Update-Database) at design time.
// Never registered in the real DI container — production code always uses CurrentTenantService.
internal class DesignTimeCurrentTenantService : ICurrentTenantService
{
    public Guid TenantId => Guid.Empty;
}