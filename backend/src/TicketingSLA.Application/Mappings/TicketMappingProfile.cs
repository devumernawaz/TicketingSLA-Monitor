using AutoMapper;
using TicketingSLA.Application.DTOs.Tickets;
using TicketingSLA.Domain.Entities;

namespace TicketingSLA.Application.Mappings;

public class TicketMappingProfile : Profile
{
    public TicketMappingProfile()
    {
        CreateMap<Ticket, TicketResponse>()
            .ForMember(dest => dest.IsBreached,
                       opt => opt.MapFrom(src => src.IsBreached(DateTime.UtcNow)));
    }
}