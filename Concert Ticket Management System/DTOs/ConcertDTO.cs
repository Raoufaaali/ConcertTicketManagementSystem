namespace Concert_Ticket_Management_System.DTOs;

/// <summary>
/// Data Transfer Object for Concert.
/// </summary>
public sealed class ConcertDTO
{
    // Preferred to use DateTimeOffset for time zone handling
    public DateTimeOffset ConcertDate { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; } = string.Empty;
}
