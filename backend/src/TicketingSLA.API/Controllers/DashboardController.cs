using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketingSLA.Application.Interfaces;
using TicketingSLA.Application.Services;

namespace TicketingSLA.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Agent")]
public class DashboardController : ControllerBase
{
    private readonly DashboardService _dashboardService;
    private readonly ICurrentTenantService _currentTenantService;

    public DashboardController(DashboardService dashboardService, ICurrentTenantService currentTenantService)
    {
        _dashboardService = dashboardService;
        _currentTenantService = currentTenantService;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var summary = await _dashboardService.GetSummaryAsync(_currentTenantService.TenantId);
        return Ok(summary);
    }
    [HttpGet("trend")]
    public async Task<IActionResult> GetTrend([FromQuery] int daysBack = 14)
    {
        var trend = await _dashboardService.GetDailyTrendAsync(_currentTenantService.TenantId, daysBack);
        return Ok(trend);
    }

}