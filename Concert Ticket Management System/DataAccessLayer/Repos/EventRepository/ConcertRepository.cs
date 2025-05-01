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

    public Task DeleteEventAsync(int concertId, CancellationToken cancellationToken)
    {
        // This method doesn't differntiate between a successful and unsuccessful deletion. It can but it doesnt
        var isDeleted = _db.TryRemove(concertId, out var concert);
        if (isDeleted)
        {
            _logger.LogInformation($"Concert with ID {concertId} deleted from the database.");
        }
        else
        {
            // This is where we would maybe return a different status code. 
            _logger.LogWarning($"Concert with ID {concertId} not found in the database.");
        }
        return Task.CompletedTask;
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
        return Task.FromResult<Concert>(null);
    }

    public Task<Concert>? UpdateEventAsync(Concert concert, CancellationToken cancellationToken)
    {
        // Check if the concert exists in the dictionary
        if (_db.ContainsKey(concert.Id))
        {
            // Update the concert in the dictionary
            _db[concert.Id] = concert;
            _logger.LogInformation($"Concert with ID {concert.Id} updated in the database.");
            return Task.FromResult<Concert?>(concert);
        }
        else
        {
            // Handle the case where the concert does not exist
            _logger.LogWarning($"Concert with ID {concert.Id} not found in the database.");
        }
        return Task.FromResult<Concert?>(null); // Return null if the concert could not be updated
    }
}
