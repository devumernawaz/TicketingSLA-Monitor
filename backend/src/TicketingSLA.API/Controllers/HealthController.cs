using Microsoft.AspNetCore.Mvc;
using TicketingSLA.Application.Interfaces;

namespace TicketingSLA.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly ISystemInfoService _systemInfoService;

    public HealthController(ISystemInfoService systemInfoService)
    {
        _systemInfoService = systemInfoService;
    }

    [HttpGet]
    public IActionResult Get() => Ok(new { message = _systemInfoService.GetStatus() });
}