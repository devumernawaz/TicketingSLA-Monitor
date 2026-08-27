namespace TicketingSLA.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(Guid userId, string email, string displayName, string role, Guid tenantId);
}
