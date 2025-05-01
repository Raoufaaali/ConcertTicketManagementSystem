namespace Concert_Ticket_Management_System.Domain.Entities;

/// <summary>
/// Represents a concert event.
/// </summary>
public sealed class Concert
{
    public int Id { get; set; }

    // Preferred to use DateTimeOffset for time zone handling
    public DateTimeOffset ConcertDate { get; set; } 

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; } = string.Empty;

    public int TotalCapacity { get; set; }

    public int AvailableCapacity { get; set; } // Remaining seats

    public Dictionary<TicketType, decimal> TicketPricing { get; set; } = []; // e.g., {"TicketType.VIP": 100.0, "TicketType.General": 50.0}
}
