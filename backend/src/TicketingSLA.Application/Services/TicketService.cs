using AutoMapper;
using FluentValidation;
using TicketingSLA.Application.DTOs.Tickets;
using TicketingSLA.Application.Interfaces;
using TicketingSLA.Domain.Entities;
using TicketingSLA.Domain.Enums;
using TicketingSLA.Shared.Common;

namespace TicketingSLA.Application.Services;

public class TicketService
{
    private readonly ITicketRepository _ticketRepository;
    private readonly ISLAPolicyRepository _slaPolicyRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IValidator<CreateTicketRequest> _createValidator;
    private readonly IValidator<UpdateTicketRequest> _updateValidator;
    private readonly IValidator<AssignTicketRequest> _assignValidator;
    private readonly IMapper _mapper;

    public TicketService(
        ITicketRepository ticketRepository,
        ISLAPolicyRepository slaPolicyRepository,
        ICurrentUserService currentUserService,
        IValidator<CreateTicketRequest> createValidator,
        IValidator<UpdateTicketRequest> updateValidator,
        IValidator<AssignTicketRequest> assignValidator,
        IMapper mapper)
    {
        _ticketRepository = ticketRepository;
        _slaPolicyRepository = slaPolicyRepository;
        _currentUserService = currentUserService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _assignValidator = assignValidator;
        _mapper = mapper;
    }

    public async Task<Result<TicketResponse>> CreateTicketAsync(CreateTicketRequest request)
    {
        var validation = await _createValidator.ValidateAsync(request);
        if (!validation.IsValid)
            return Result<TicketResponse>.Failure(FormatErrors(validation));

        var slaPolicy = await _slaPolicyRepository.GetByPriorityAsync(request.Priority);
        if (slaPolicy is null)
            return Result<TicketResponse>.Failure($"No SLA policy configured for priority '{request.Priority}'.");

        Ticket ticket;
        try
        {
            ticket = new Ticket(request.Title, request.Description, slaPolicy, _currentUserService.UserId);
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
        if (ticket is null || !CanAccess(ticket))
            return Result<TicketResponse>.Failure("Ticket not found.");

        return Result<TicketResponse>.Success(_mapper.Map<TicketResponse>(ticket));
    }

    public async Task<Result<PagedTicketResponse>> GetAllAsync(
        TicketStatus? status, TicketPriority? priority, int page, int pageSize)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 200);

        var createdByUserId = _currentUserService.Role == "Client" ? _currentUserService.UserId : (Guid?)null;

        var (items, totalCount) = await _ticketRepository.GetPagedAsync(status, priority, createdByUserId, page, pageSize);

        return Result<PagedTicketResponse>.Success(new PagedTicketResponse
        {
            Items = _mapper.Map<IEnumerable<TicketResponse>>(items),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
        });
    }

    public async Task<Result<TicketResponse>> UpdateTicketAsync(Guid id, UpdateTicketRequest request)
    {
        var validation = await _updateValidator.ValidateAsync(request);
        if (!validation.IsValid)
            return Result<TicketResponse>.Failure(FormatErrors(validation));

        var ticket = await _ticketRepository.GetByIdAsync(id);
        if (ticket is null || !CanAccess(ticket))
            return Result<TicketResponse>.Failure("Ticket not found.");

        try
        {
            ticket.Update(request.Title, request.Description);
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            return Result<TicketResponse>.Failure(ex.Message);
        }

        _ticketRepository.Update(ticket);
        await _ticketRepository.SaveChangesAsync();

        return Result<TicketResponse>.Success(_mapper.Map<TicketResponse>(ticket));
    }

    public async Task<Result> DeleteTicketAsync(Guid id)
    {
        var ticket = await _ticketRepository.GetByIdAsync(id);
        if (ticket is null)
            return Result.Failure("Ticket not found.");

        _ticketRepository.Delete(ticket);
        await _ticketRepository.SaveChangesAsync();

        return Result.Success();
    }

    private bool CanAccess(Ticket ticket) =>
        _currentUserService.Role != "Client" || ticket.CreatedByUserId == _currentUserService.UserId;

    private static string FormatErrors(FluentValidation.Results.ValidationResult validation) =>
        string.Join(" ", validation.Errors.Select(e => e.ErrorMessage));

    public async Task<Result<TicketResponse>> AssignTicketAsync(Guid ticketId, AssignTicketRequest request)
    {
        var validation = await _assignValidator.ValidateAsync(request);
        if (!validation.IsValid)
            return Result<TicketResponse>.Failure(FormatErrors(validation));

        var ticket = await _ticketRepository.GetByIdAsync(ticketId);
        if (ticket is null)
            return Result<TicketResponse>.Failure("Ticket not found.");

        try
        {
            ticket.AssignTo(request.AgentId);
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