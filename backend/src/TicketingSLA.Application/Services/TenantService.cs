using AutoMapper;
using FluentValidation;
using TicketingSLA.Application.DTOs.Tenants;
using TicketingSLA.Application.Interfaces;
using TicketingSLA.Domain.Entities;
using TicketingSLA.Shared.Common;

namespace TicketingSLA.Application.Services;

public class TenantService
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IValidator<CreateTenantRequest> _createValidator;
    private readonly IMapper _mapper;

    public TenantService(
        ITenantRepository tenantRepository,
        IValidator<CreateTenantRequest> createValidator,
        IMapper mapper)
    {
        _tenantRepository = tenantRepository;
        _createValidator = createValidator;
        _mapper = mapper;
    }

    public async Task<Result<TenantResponse>> CreateAsync(CreateTenantRequest request)
    {
        var validation = await _createValidator.ValidateAsync(request);
        if (!validation.IsValid)
            return Result<TenantResponse>.Failure(string.Join(" ", validation.Errors.Select(e => e.ErrorMessage)));

        Tenant tenant;
        try
        {
            tenant = new Tenant(request.Name);
        }
        catch (ArgumentException ex)
        {
            return Result<TenantResponse>.Failure(ex.Message);
        }

        await _tenantRepository.AddAsync(tenant);
        await _tenantRepository.SaveChangesAsync();

        return Result<TenantResponse>.Success(_mapper.Map<TenantResponse>(tenant));
    }

    public async Task<Result<IEnumerable<TenantResponse>>> GetActiveAsync()
    {
        var tenants = await _tenantRepository.GetAllAsync();
        var active = tenants.Where(t => t.IsActive);

        return Result<IEnumerable<TenantResponse>>.Success(_mapper.Map<IEnumerable<TenantResponse>>(active));
    }
}
