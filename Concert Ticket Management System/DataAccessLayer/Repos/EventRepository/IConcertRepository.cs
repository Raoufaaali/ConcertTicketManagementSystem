using Concert_Ticket_Management_System.Domain.Entities;
using Concert_Ticket_Management_System.DTOs;

namespace Concert_Ticket_Management_System.DataAccessLayer.Repos.EventRepository;

public interface IConcertRepository
{
    /// <summary>
    /// Retrieves all concert events.
    /// </summary>
    /// <returns>A list of all current concerts</returns>
    Task<IEnumerable<Concert>> GetAllEventsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a concert event by its id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<Concert>? GetEventByIdAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    /// Adds a new concert event.
    /// </summary>
    /// <param name="concert"></param>
    /// <returns></returns>
    Task<Concert>? AddEventAsync(Concert concert, CancellationToken cancellationToken);

    /// <summary>
    /// Updates an existing concert event.
    /// </summary>
    /// <param name="concert"></param>
    /// <returns></returns>
    Task<Concert>? UpdateEventAsync(Concert concert, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes a concert event by its id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task DeleteEventAsync(int id, CancellationToken cancellationToken);
}
