using Microsoft.AspNetCore.Identity;
using TicketingSLA.Application.DTOs.Users;
using TicketingSLA.Application.Interfaces;
using TicketingSLA.Infrastructure.Identity;

namespace TicketingSLA.Infrastructure.Services;

public class UserDirectoryService : IUserDirectoryService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserDirectoryService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IEnumerable<UserResponse>> GetUsersByRoleAsync(Guid tenantId, string role)
    {
        var usersInRole = await _userManager.GetUsersInRoleAsync(role);

        return usersInRole
            .Where(u => u.TenantId == tenantId)
            .Select(u => new UserResponse
            {
                Id = u.Id,
                Email = u.Email ?? string.Empty,
                DisplayName = u.DisplayName,
                Role = role,
            });
    }
}
