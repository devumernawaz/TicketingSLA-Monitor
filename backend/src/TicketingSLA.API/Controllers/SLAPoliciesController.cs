using Microsoft.AspNetCore.Mvc;
using TicketingSLA.Application.DTOs.SLAPolicies;
using TicketingSLA.Application.Services;

namespace TicketingSLA.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SLAPoliciesController : ControllerBase
{
    private readonly SLAPolicyService _slaPolicyService;

    public SLAPoliciesController(SLAPolicyService slaPolicyService)
    {
        _slaPolicyService = slaPolicyService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSLAPolicyRequest request)
    {
        var result = await _slaPolicyService.CreateAsync(request);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _slaPolicyService.GetAllAsync();
        return Ok(result.Value);
    }
}