using AutoMapper;
using Concert_Ticket_Management_System.Domain.Entities;
using Concert_Ticket_Management_System.DTOs;

namespace Concert_Ticket_Management_System.Shared;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Map ConcertDTO to Concert
        CreateMap<ConcertDTO, Concert>();
    }
}