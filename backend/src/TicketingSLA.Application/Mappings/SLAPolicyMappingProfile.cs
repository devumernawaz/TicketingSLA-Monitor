using AutoMapper;
using TicketingSLA.Application.DTOs.SLAPolicies;
using TicketingSLA.Domain.Entities;

namespace TicketingSLA.Application.Mappings;

public class SLAPolicyMappingProfile : Profile
{
    public SLAPolicyMappingProfile()
    {
        CreateMap<SLAPolicy, SLAPolicyResponse>();
    }
}