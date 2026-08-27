using AutoMapper;
using TicketingSLA.Application.DTOs.Tenants;
using TicketingSLA.Domain.Entities;

namespace TicketingSLA.Application.Mappings;

public class TenantMappingProfile : Profile
{
    public TenantMappingProfile()
    {
        CreateMap<Tenant, TenantResponse>();
    }
}
