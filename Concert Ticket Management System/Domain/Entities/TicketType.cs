namespace Concert_Ticket_Management_System.Domain.Entities;

public enum TicketType : byte
{
    /// <summary>
    /// Unknown ticket type.
    /// </summary>
    Unknown,

    /// <summary>
    /// VIP ticket type.
    /// </summary>
    VIP,

    /// <summary>
    /// General ticket type.
    /// </summary>
    General,
    /// <summary>
    /// Early Bird ticket type.
    /// </summary>
    EarlyBird,
    /// <summary>
    /// Student ticket type.
    /// </summary>
    Student
}
