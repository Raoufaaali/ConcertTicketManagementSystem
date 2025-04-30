namespace Concert_Ticket_Management_System.Domain.Entities;

/// <summary>
/// Represents a concert event.
/// </summary>
public sealed class Concert
{
    public int Id { get; set; }

    // Preferred to use DateTimeOffset for time zone handling
    public DateTimeOffset StartDate { get; set; } 

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; } = string.Empty;
}
