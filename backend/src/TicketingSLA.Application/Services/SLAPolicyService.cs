using AutoMapper;
using FluentValidation;
using TicketingSLA.Application.DTOs.SLAPolicies;
using TicketingSLA.Application.Interfaces;
using TicketingSLA.Domain.Entities;
using TicketingSLA.Shared.Common;

namespace TicketingSLA.Application.Services;

public class SLAPolicyService
{
    private readonly ISLAPolicyRepository _slaPolicyRepository;
    private readonly IValidator<CreateSLAPolicyRequest> _createValidator;
    private readonly IValidator<UpdateSLAPolicyRequest> _updateValidator;
    private readonly IMapper _mapper;

    public SLAPolicyService(
        ISLAPolicyRepository slaPolicyRepository,
        IValidator<CreateSLAPolicyRequest> createValidator,
        IValidator<UpdateSLAPolicyRequest> updateValidator,
        IMapper mapper)
    {
        _slaPolicyRepository = slaPolicyRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _mapper = mapper;
    }

    public async Task<Result<SLAPolicyResponse>> CreateAsync(CreateSLAPolicyRequest request)
    {
        var validation = await _createValidator.ValidateAsync(request);
        if (!validation.IsValid)
            return Result<SLAPolicyResponse>.Failure(FormatErrors(validation));

        SLAPolicy policy;
        try
        {
            policy = new SLAPolicy(request.Name, request.Priority, request.ResponseTimeHours);
        }
        catch (ArgumentException ex)
        {
            return Result<SLAPolicyResponse>.Failure(ex.Message);
        }

        await _slaPolicyRepository.AddAsync(policy);
        await _slaPolicyRepository.SaveChangesAsync();

        return Result<SLAPolicyResponse>.Success(_mapper.Map<SLAPolicyResponse>(policy));
    }

    public async Task<Result<IEnumerable<SLAPolicyResponse>>> GetAllAsync()
    {
        var policies = await _slaPolicyRepository.GetAllAsync();
        return Result<IEnumerable<SLAPolicyResponse>>.Success(_mapper.Map<IEnumerable<SLAPolicyResponse>>(policies));
    }

    public async Task<Result<SLAPolicyResponse>> GetByIdAsync(Guid id)
    {
        var policy = await _slaPolicyRepository.GetByIdAsync(id);
        if (policy is null)
            return Result<SLAPolicyResponse>.Failure("SLA policy not found.");

        return Result<SLAPolicyResponse>.Success(_mapper.Map<SLAPolicyResponse>(policy));
    }

    public async Task<Result<SLAPolicyResponse>> UpdateAsync(Guid id, UpdateSLAPolicyRequest request)
    {
        var validation = await _updateValidator.ValidateAsync(request);
        if (!validation.IsValid)
            return Result<SLAPolicyResponse>.Failure(FormatErrors(validation));

        var policy = await _slaPolicyRepository.GetByIdAsync(id);
        if (policy is null)
            return Result<SLAPolicyResponse>.Failure("SLA policy not found.");

        try
        {
            policy.Update(request.Name, request.ResponseTimeHours);
        }
        catch (ArgumentException ex)
        {
            return Result<SLAPolicyResponse>.Failure(ex.Message);
        }

        _slaPolicyRepository.Update(policy);
        await _slaPolicyRepository.SaveChangesAsync();

        return Result<SLAPolicyResponse>.Success(_mapper.Map<SLAPolicyResponse>(policy));
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var policy = await _slaPolicyRepository.GetByIdAsync(id);
        if (policy is null)
            return Result.Failure("SLA policy not found.");

        _slaPolicyRepository.Delete(policy);
        await _slaPolicyRepository.SaveChangesAsync();

        return Result.Success();
    }

    private static string FormatErrors(FluentValidation.Results.ValidationResult validation) =>
        string.Join(" ", validation.Errors.Select(e => e.ErrorMessage));
}