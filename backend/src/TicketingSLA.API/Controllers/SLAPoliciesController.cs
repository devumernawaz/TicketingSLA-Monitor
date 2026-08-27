using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketingSLA.Application.DTOs.SLAPolicies;
using TicketingSLA.Application.Services;

namespace TicketingSLA.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SLAPoliciesController : ControllerBase
{
    private readonly SLAPolicyService _slaPolicyService;

    public SLAPoliciesController(SLAPolicyService slaPolicyService)
    {
        _slaPolicyService = slaPolicyService;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
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

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _slaPolicyService.GetByIdAsync(id);
        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(new { error = result.Error });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSLAPolicyRequest request)
    {
        var result = await _slaPolicyService.UpdateAsync(id, request);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error });
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _slaPolicyService.DeleteAsync(id);
        return result.IsSuccess
            ? NoContent()
            : BadRequest(new { error = result.Error });
    }
}