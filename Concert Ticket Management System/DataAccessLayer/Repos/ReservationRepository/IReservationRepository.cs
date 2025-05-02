using Concert_Ticket_Management_System.Domain.Entities;
using Concert_Ticket_Management_System.Shared;

namespace Concert_Ticket_Management_System.DataAccessLayer.Repos.ReservationRepository;

public interface IReservationRepository
{
    Task<Result<Reservation>> AddReservationAsync(Reservation reservation, Concert concert, CancellationToken cancellationToken);

    Task<Result<Reservation>> ConfirmReservationAsync(int reservationId, CancellationToken cancellationToken);

    Task<Reservation?> GetReservationAsync(int reservationId, int concertId, CancellationToken cancellationToken);
}
