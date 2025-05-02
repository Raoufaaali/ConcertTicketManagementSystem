using Concert_Ticket_Management_System.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Concert_Ticket_Management_System.DTOs;

public class ReservationResponse
{
    public int ConcertId { get; set; }

    public TicketType TicketType { get; set; } // I should make this a collection of TicketType and Quantity pairs but this is not a real implementation :)

    public int Quantity { get; set; }

    public DateTimeOffset ReservedAt { get; set; }

    public DateTimeOffset ReservationExpiry { get; set; }

    public ReservationStatus ReservationStatus { get; set; }
}