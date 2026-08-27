using TicketingSLA.Application.DTOs.Users;

namespace TicketingSLA.Application.Interfaces;

public interface IUserDirectoryService
{
    Task<IEnumerable<UserResponse>> GetUsersByRoleAsync(Guid tenantId, string role);
}
