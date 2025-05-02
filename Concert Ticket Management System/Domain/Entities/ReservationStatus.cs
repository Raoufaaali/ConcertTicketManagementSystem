namespace Concert_Ticket_Management_System.Domain.Entities;

public enum ReservationStatus : byte
{
    Unknown = 0,
    Pending = 1,
    Confirmed = 2,
    Expired = 3,
    Canceled = 4,
    Refunded = 5,
}
