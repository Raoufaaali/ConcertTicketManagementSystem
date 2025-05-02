using AutoMapper;
using Concert_Ticket_Management_System.Domain.Entities;
using Concert_Ticket_Management_System.DTOs;

namespace Concert_Ticket_Management_System.Shared.Mappers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Map ConcertDTO to Concert
        CreateMap<ConcertDTO, Concert>()
            .ForMember(dest => dest.AvailableCapacity, opt => opt.MapFrom(src => src.TotalCapacity));

        // Map ReservationDTO to Reservation
        CreateMap<ReservationRequest, Reservation>()
            .ForMember(dest => dest.TicketType, opt => opt.MapFrom(src => src.TicketType))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
            .ForMember(dest => dest.ConcertId, opt => opt.MapFrom(src => src.ConcertId));

    }
}