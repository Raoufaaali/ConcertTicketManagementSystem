using Concert_Ticket_Management_System.Domain.Entities;
using System.Collections.Concurrent;

namespace Concert_Ticket_Management_System.DataAccessLayer.Repos.EventRepository;

public class ConcertRepository : IConcertRepository
{
    // Used a dictionary to simulate a database for the sake of this example coz why not 
    private readonly ConcurrentDictionary<int, Concert> _db = [];
    private int _currentId = 0; // Counter for auto-incrementing keys
    ILogger<ConcertRepository> _logger;

    public ConcertRepository(ILogger<ConcertRepository> logger)
    {
        _logger = logger;
    }

    public Task<Concert>? AddEventAsync(Concert concert, CancellationToken cancellationToken)
    {
        // This is not an actual async method, but for the sake of the example, we will keep it as such.
        // In a real-world scenario, you would use async methods to interact with a database.

        // Generate a unique key in a thread-safe manner
        int newId = Interlocked.Increment(ref _currentId);

        // Set the concert's Id
        concert.Id = newId;

        if (_db.TryAdd(newId, concert))
        {
            // Successfully added the concert to the dictionary AKA the database
            _logger.LogInformation($"Concert with ID {newId} added to the database.");
            return Task.FromResult(concert);
        }
        else
        {
            // Handle the case where the concert could not be added
        }

        return null; // Return null if the concert could not be added
    }

    public Task DeleteEventAsync(int eventId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Concert>> GetAllEventsAsync(CancellationToken cancellationToken)
    {
        // Convert the values of the dictionary to a list and return it as a Task
        var concerts = _db.Values.ToList();
        return Task.FromResult<IEnumerable<Concert>>(concerts);
    }

    public Task<Concert>? GetEventByIdAsync(int eventId, CancellationToken cancellationToken)
    {
        // Try to get the concert from the dictionary
        if (_db.TryGetValue(eventId, out var concert))
        {
            return Task.FromResult(concert);
        }
        return null;
    }

    public Task UpdateEventAsync(Concert concert, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
