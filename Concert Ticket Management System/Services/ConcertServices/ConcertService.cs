using Concert_Ticket_Management_System.DataAccessLayer.Repos.EventRepository;
using Concert_Ticket_Management_System.Domain.Entities;
using Concert_Ticket_Management_System.DTOs;

namespace Concert_Ticket_Management_System.Services.ConcertServices;

public sealed class ConcertService : IConcertService
{
    private readonly IConcertRepository _concertRepository;
    private readonly ILogger<ConcertService> _logger;

    public ConcertService(
        IConcertRepository concertRepository,
        ILogger<ConcertService> logger)
    {
        _concertRepository = concertRepository;
        _logger = logger;
    }

    public async Task<Concert>? AddConcertAsync(ConcertDTO concert, CancellationToken cancellationToken)
    {
        var entity = new Concert
        {
            Name = concert.Name,
            Description = concert.Description,
            StartDate = concert.ConcertDate
        };
        return await _concertRepository.AddEventAsync(entity, cancellationToken);
    }

    public Task DeleteConcertAsync(int concertId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Concert>> GetAllConcertsAsync(CancellationToken cancellationToken)
    {
        return await _concertRepository.GetAllEventsAsync(cancellationToken);
    }

    public Task<Concert> GetConcertByIdAsync(int concertId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task UpdateConcertAsync(Concert concert, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
