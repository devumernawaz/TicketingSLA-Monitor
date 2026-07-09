namespace TicketingSLA.Application.Interfaces;

public interface ICurrentTenantService
{
    Guid TenantId { get; }
}