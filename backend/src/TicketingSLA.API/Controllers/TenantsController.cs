using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketingSLA.Application.DTOs.Tenants;
using TicketingSLA.Application.Services;

namespace TicketingSLA.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class TenantsController : ControllerBase
{
    private readonly TenantService _tenantService;

    public TenantsController(TenantService tenantService)
    {
        _tenantService = tenantService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _tenantService.GetActiveAsync();
        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTenantRequest request)
    {
        var result = await _tenantService.CreateAsync(request);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error });
    }
}
