using Concert_Ticket_Management_System.Domain.Entities;
using Concert_Ticket_Management_System.DTOs;

namespace Concert_Ticket_Management_System.Services.ConcertServices;

public interface IConcertService
{
    public Task<IEnumerable<Concert>> GetAllConcertsAsync(CancellationToken cancellationToken);
    public Task<Concert>? GetConcertByIdAsync(int concertId, CancellationToken cancellationToken);
    public Task<Concert>? AddConcertAsync(ConcertDTO concert, CancellationToken cancellationToken);
    public Task? UpdateConcertAsync(Concert concert, CancellationToken cancellationToken);
    public Task? DeleteConcertAsync(int concertId, CancellationToken cancellationToken);
}
