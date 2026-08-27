using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketingSLA.Application.Interfaces;
using TicketingSLA.Domain.Enums;

namespace TicketingSLA.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Agent")]
public class UsersController : ControllerBase
{
    private readonly IUserDirectoryService _userDirectoryService;
    private readonly ICurrentTenantService _currentTenantService;

    public UsersController(IUserDirectoryService userDirectoryService, ICurrentTenantService currentTenantService)
    {
        _userDirectoryService = userDirectoryService;
        _currentTenantService = currentTenantService;
    }

    [HttpGet]
    public async Task<IActionResult> GetByRole([FromQuery] UserRole? role)
    {
        if (role is null)
            return BadRequest(new { error = "The 'role' query parameter is required." });

        var users = await _userDirectoryService.GetUsersByRoleAsync(_currentTenantService.TenantId, role.ToString()!);
        return Ok(users);
    }
}
