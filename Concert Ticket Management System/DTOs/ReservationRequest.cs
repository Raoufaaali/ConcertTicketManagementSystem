using Concert_Ticket_Management_System.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Concert_Ticket_Management_System.DTOs;

public class ReservationRequest
{
    [Required(ErrorMessage = "ConcertId is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "ConcertId must be a positive integer.")]
    public int ConcertId { get; set; }

    [Required(ErrorMessage = "TicketType is required.")]
    [EnumDataType(typeof(TicketType), ErrorMessage = "Invalid TicketType.")]

    public TicketType TicketType { get; set; }

    [Required(ErrorMessage = "Quantity is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
    public int Quantity { get; set; }

}