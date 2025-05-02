namespace Concert_Ticket_Management_System.Domain.Entities;

public sealed class Reservation
{
    private int _reservationExpiraryUpdateCounter; // this keeps track of how many times the reservation expiry has been updated

    public int Id { get; set; }

    public int ConcertId { get; set; }

    public TicketType TicketType { get; set; }

    public int Quantity { get; set; }

    public DateTimeOffset ReservedAt { get; } // Read only

    public DateTimeOffset ReservationExpiry { get; private set; } // This is a controlled property. I want to control how it is set.

    public  ReservationStatus ReservationStatus { get; private set; } // Same here, I determine this

    public void UpdateReservationStatue(ReservationStatus status)
    { 
        // validate business rules
        ReservationStatus = status;
    }

    public void SetReservationExpiry(DateTimeOffset expiray)
    {
        // I feel like this should be written-to only once. Hence checking the boolean before updating. 
        if (_reservationExpiraryUpdateCounter is 0)
        {
            ReservationExpiry = expiray;
            _reservationExpiraryUpdateCounter++;
        }

        else
        {
            throw new InvalidOperationException("Reservation expiry can only be set once.");
        }
    }

}