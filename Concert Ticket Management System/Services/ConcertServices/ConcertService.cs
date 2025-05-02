using AutoMapper;
using Concert_Ticket_Management_System.DataAccessLayer.Repos.EventRepository;
using Concert_Ticket_Management_System.DataAccessLayer.Repos.ReservationRepository;
using Concert_Ticket_Management_System.Domain.Entities;
using Concert_Ticket_Management_System.DTOs;
using Concert_Ticket_Management_System.Shared;

namespace Concert_Ticket_Management_System.Services.ConcertServices;

public sealed class ConcertService : IConcertService
{
    private readonly IConcertRepository _concertRepository;
    private readonly IReservationRepository _reservationRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ConcertService> _logger;

    public ConcertService(
        IConcertRepository concertRepository,
        IReservationRepository reservationRepository,
        IMapper mapper,
        ILogger<ConcertService> logger)
    {
        _concertRepository = concertRepository;
        _reservationRepository = reservationRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Concert>? AddConcertAsync(ConcertDTO concert, CancellationToken cancellationToken)
    {
        // This mapper profile correctly sets the AvailabeCapacity to equal the TotalCapacity
        var enitity = _mapper.Map<Concert>(concert);
        return await _concertRepository.AddEventAsync(enitity, cancellationToken).ConfigureAwait(false);
    }

    public async Task DeleteConcertAsync(int concertId, CancellationToken cancellationToken)
    { 
        await _concertRepository.DeleteEventAsync(concertId, cancellationToken).ConfigureAwait(false);
    }

    public async Task<IEnumerable<Concert>> GetAllConcertsAsync(CancellationToken cancellationToken)
    {
        return await _concertRepository.GetAllEventsAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<Concert>? GetConcertByIdAsync(int concertId, CancellationToken cancellationToken)
    {
        var concert = await _concertRepository.GetEventByIdAsync(concertId, cancellationToken).ConfigureAwait(false);
        if (concert == null)
        {
            _logger.LogWarning("Concert with ID {Id} not found.", concertId);
        }
        return concert;
    }

    public async Task<Result<int>> UpdateAvailableCapacityAsync(int concertId, ManageCapacityRequest request, CancellationToken cancellationToken)
    {
        /*
         * Validate the concert exists
         * Validate business rules
         * return errors if any
         * call db to update the available capacity
         */

        // Check if the concert exists in the dictionary
        var existingConcert = await _concertRepository.GetEventByIdAsync(concertId, cancellationToken).ConfigureAwait(false);
        if (existingConcert == null)
        {
            return Result<int>.Failure(new[] { "Concert not found." });
        }

        var rejectionReasons = ValidateAvailableCapacityRules(existingConcert, request.NewAvailableCapacity);

        if (rejectionReasons.Any())
        {
            return Result<int>.Failure(rejectionReasons);
        }

        // Update the available capacity
        await _concertRepository.UpdateAvailableCapacityAsync(concertId, request.NewAvailableCapacity, cancellationToken).ConfigureAwait(false);
        return Result<int>.Success(0);
    }

    private IEnumerable<string> ValidateAvailableCapacityRules(Concert existingConcert, int newAvailableCapacity)
    {
        if (newAvailableCapacity <= 0)
        {
            yield return "Available capacity must be greater than zero.";
        }
        if (newAvailableCapacity > existingConcert.TotalCapacity)
        {
            yield return "Available capacity cannot exceed total capacity.";
        }
        yield break;
    }

    public async Task<Result<Concert>> UpdateConcertAsync(int concertId, ConcertDTO concertDto, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<Concert>(concertDto);
        entity.Id = concertId;

        // Validate the entity exists in the db before proceeding
        var existingConcert = await _concertRepository.GetEventByIdAsync(concertId, cancellationToken).ConfigureAwait(false);
        if (existingConcert == null)
        {
            return Result<Concert>.Failure(["Concert not found."]);
        }

        var rejectionReasons = ValidateBusinessRules(entity);
        if (rejectionReasons.Any())
        {
            return Result<Concert>.Failure(rejectionReasons);
        }

        var updatedConcert = await _concertRepository.UpdateEventAsync(entity, cancellationToken).ConfigureAwait(false);

        if (updatedConcert == null)
        {
            return Result<Concert>.Failure(new[] { "Concert not found." });
        }

        return Result<Concert>.Success(updatedConcert);
    }

    private IEnumerable<string> ValidateBusinessRules(Concert newConcert)
    {
        if (newConcert.ConcertDate < DateTime.UtcNow)
        {
            yield return "Concert date cannot be in the past.";
        }

        yield break;
    }

    public async Task<Result<Reservation>> ReserveTicketsAsync(ReservationRequest request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return Result<Reservation>.Failure(new[] { "Request cannot be null." });
        }

        // Get the count and update the capacity for later comparison
        var concert = await _concertRepository.GetEventByIdAsync(request.ConcertId, cancellationToken).ConfigureAwait(false);
        if (concert == null)
        {
            return Result<Reservation>.Failure(new[] { "Concert not found." });
        }

        if (!concert.TicketPricing.ContainsKey(request.TicketType))
        {
            return Result<Reservation>.Failure(new[] { "Invalid ticket type." });
        }

        var entity = _mapper.Map<Reservation>(request);

        // Hardcoded for now. This should be a configuration value
        var ExpireAfterSeconds = 60; // 1 minute

        entity.SetReservationExpiry(CalculateExpiraryDate(ExpireAfterSeconds));
        entity.UpdateReservationStatue(ReservationStatus.Pending);

        // Not a big fan of this try and catch but I need to revert the available capacity if the reservation fails
        // Typically this is done in a transaction. This is the price I pay for opting to a dictionary as an in-memmory db

        try
        {

            var result = await _reservationRepository.AddReservationAsync(entity, concert, cancellationToken).ConfigureAwait(false);
            var newCapacity = concert.AvailableCapacity - entity.Quantity;
            await _concertRepository.UpdateAvailableCapacityAsync(concert.Id, newCapacity, cancellationToken).ConfigureAwait(false);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to reserve tickets for concert {ConcertId}", concert.Id);

            // Rollback available capacity
            concert.AvailableCapacity += request.Quantity;
            await _concertRepository.UpdateAvailableCapacityAsync(concert.Id, concert.AvailableCapacity, cancellationToken).ConfigureAwait(false);

            throw;
        }
    }

    public async Task<Reservation?> GetReservationByIdAsync(int reservationId, int concertId,  CancellationToken cancellationToken)
    {
        var reservation = await _reservationRepository.GetReservationAsync(reservationId, concertId, cancellationToken).ConfigureAwait(false);
        return reservation;
    }

    private DateTimeOffset CalculateExpiraryDate(int seconds)
    {
        return DateTimeOffset.UtcNow.AddSeconds(seconds);
    }

}
