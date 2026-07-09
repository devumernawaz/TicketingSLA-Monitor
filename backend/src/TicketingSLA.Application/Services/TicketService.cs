using AutoMapper;
using TicketingSLA.Application.DTOs.Tickets;
using TicketingSLA.Application.Interfaces;
using TicketingSLA.Domain.Entities;
using TicketingSLA.Shared.Common;

namespace TicketingSLA.Application.Services;

public class TicketService
{
    private readonly ITicketRepository _ticketRepository;
    private readonly ISLAPolicyRepository _slaPolicyRepository;
    private readonly IMapper _mapper;

    public TicketService(
        ITicketRepository ticketRepository,
        ISLAPolicyRepository slaPolicyRepository,
        IMapper mapper)
    {
        _ticketRepository = ticketRepository;
        _slaPolicyRepository = slaPolicyRepository;
        _mapper = mapper;
    }

    public async Task<Result<TicketResponse>> CreateTicketAsync(CreateTicketRequest request)
    {
        var slaPolicy = await _slaPolicyRepository.GetByPriorityAsync(request.Priority);
        if (slaPolicy is null)
            return Result<TicketResponse>.Failure($"No SLA policy configured for priority '{request.Priority}'.");

        Ticket ticket;
        try
        {
            ticket = new Ticket(request.Title, request.Description, slaPolicy);
        }
        catch (ArgumentException ex)
        {
            return Result<TicketResponse>.Failure(ex.Message);
        }

        await _ticketRepository.AddAsync(ticket);
        await _ticketRepository.SaveChangesAsync();

        return Result<TicketResponse>.Success(_mapper.Map<TicketResponse>(ticket));
    }

    public async Task<Result<TicketResponse>> GetByIdAsync(Guid id)
    {
        var ticket = await _ticketRepository.GetByIdAsync(id);
        if (ticket is null)
            return Result<TicketResponse>.Failure("Ticket not found.");

        return Result<TicketResponse>.Success(_mapper.Map<TicketResponse>(ticket));
    }

    public async Task<Result<IEnumerable<TicketResponse>>> GetAllAsync()
    {
        var tickets = await _ticketRepository.GetAllAsync();
        return Result<IEnumerable<TicketResponse>>.Success(_mapper.Map<IEnumerable<TicketResponse>>(tickets));
    }

    public async Task<Result<TicketResponse>> AssignTicketAsync(Guid ticketId, Guid agentId)
    {
        var ticket = await _ticketRepository.GetByIdAsync(ticketId);
        if (ticket is null)
            return Result<TicketResponse>.Failure("Ticket not found.");

        try
        {
            ticket.AssignTo(agentId);
        }
        catch (InvalidOperationException ex)
        {
            return Result<TicketResponse>.Failure(ex.Message);
        }

        await _ticketRepository.SaveChangesAsync();
        return Result<TicketResponse>.Success(_mapper.Map<TicketResponse>(ticket));
    }

    public async Task<Result<TicketResponse>> CloseTicketAsync(Guid ticketId)
    {
        var ticket = await _ticketRepository.GetByIdAsync(ticketId);
        if (ticket is null)
            return Result<TicketResponse>.Failure("Ticket not found.");

        ticket.Close();
        await _ticketRepository.SaveChangesAsync();

        return Result<TicketResponse>.Success(_mapper.Map<TicketResponse>(ticket));
    }
}