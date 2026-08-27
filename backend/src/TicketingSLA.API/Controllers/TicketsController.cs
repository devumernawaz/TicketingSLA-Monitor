using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketingSLA.Application.DTOs.Tickets;
using TicketingSLA.Application.Services;
using TicketingSLA.Domain.Enums;

namespace TicketingSLA.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TicketsController : ControllerBase
{
    private readonly TicketService _ticketService;

    public TicketsController(TicketService ticketService)
    {
        _ticketService = ticketService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTicketRequest request)
    {
        var result = await _ticketService.CreateTicketAsync(request);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value)
            : BadRequest(new { error = result.Error });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _ticketService.GetByIdAsync(id);
        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(new { error = result.Error });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] TicketStatus? status,
        [FromQuery] TicketPriority? priority,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var result = await _ticketService.GetAllAsync(status, priority, page, pageSize);
        return Ok(result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTicketRequest request)
    {
        var result = await _ticketService.UpdateTicketAsync(id, request);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error });
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _ticketService.DeleteTicketAsync(id);
        return result.IsSuccess
            ? NoContent()
            : BadRequest(new { error = result.Error });
    }

    [HttpPatch("{id:guid}/assign")]
    [Authorize(Roles = "Admin,Agent")]
    public async Task<IActionResult> Assign(Guid id, [FromBody] AssignTicketRequest request)
    {
        var result = await _ticketService.AssignTicketAsync(id, request.AgentId);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error });
    }

    [HttpPatch("{id:guid}/close")]
    [Authorize(Roles = "Admin,Agent")]
    public async Task<IActionResult> Close(Guid id)
    {
        var result = await _ticketService.CloseTicketAsync(id);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error });
    }
}