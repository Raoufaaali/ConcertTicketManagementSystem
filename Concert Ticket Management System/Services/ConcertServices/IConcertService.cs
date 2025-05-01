using Concert_Ticket_Management_System.Domain.Entities;
using Concert_Ticket_Management_System.DTOs;
using Concert_Ticket_Management_System.Shared;

namespace Concert_Ticket_Management_System.Services.ConcertServices;

public interface IConcertService
{
    public Task<IEnumerable<Concert>> GetAllConcertsAsync(CancellationToken cancellationToken);
    public Task<Concert>? GetConcertByIdAsync(int concertId, CancellationToken cancellationToken);
    public Task<Concert>? AddConcertAsync(ConcertDTO concert, CancellationToken cancellationToken);
    public Task<Result<Concert>> UpdateConcertAsync(int concertId, ConcertDTO concert, CancellationToken cancellationToken);
    public Task? DeleteConcertAsync(int concertId, CancellationToken cancellationToken);
    public Task<Result<int>> UpdateAvailableCapacityAsync(int concertId, ManageCapacityRequest request, CancellationToken cancellationToken);
}
