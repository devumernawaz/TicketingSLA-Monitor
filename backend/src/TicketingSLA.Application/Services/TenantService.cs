using AutoMapper;
using TicketingSLA.Application.DTOs.Tenants;
using TicketingSLA.Application.Interfaces;
using TicketingSLA.Domain.Entities;
using TicketingSLA.Shared.Common;

namespace TicketingSLA.Application.Services;

public class TenantService
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IMapper _mapper;

    public TenantService(ITenantRepository tenantRepository, IMapper mapper)
    {
        _tenantRepository = tenantRepository;
        _mapper = mapper;
    }

    public async Task<Result<TenantResponse>> CreateAsync(CreateTenantRequest request)
    {
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
