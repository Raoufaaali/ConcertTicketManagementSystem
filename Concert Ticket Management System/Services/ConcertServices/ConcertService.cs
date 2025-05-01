using AutoMapper;
using Concert_Ticket_Management_System.DataAccessLayer.Repos.EventRepository;
using Concert_Ticket_Management_System.Domain.Entities;
using Concert_Ticket_Management_System.DTOs;
using Concert_Ticket_Management_System.Shared;

namespace Concert_Ticket_Management_System.Services.ConcertServices;

public sealed class ConcertService : IConcertService
{
    private readonly IConcertRepository _concertRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ConcertService> _logger;

    public ConcertService(
        IConcertRepository concertRepository,
        IMapper mapper,
        ILogger<ConcertService> logger)
    {
        _concertRepository = concertRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Concert>? AddConcertAsync(ConcertDTO concert, CancellationToken cancellationToken)
    {
        var enitity = _mapper.Map<Concert>(concert);
        return await _concertRepository.AddEventAsync(enitity, cancellationToken);
    }

    public async Task DeleteConcertAsync(int concertId, CancellationToken cancellationToken)
    { 
        await _concertRepository.DeleteEventAsync(concertId, cancellationToken);
    }

    public async Task<IEnumerable<Concert>> GetAllConcertsAsync(CancellationToken cancellationToken)
    {
        return await _concertRepository.GetAllEventsAsync(cancellationToken);
    }

    public async Task<Concert>? GetConcertByIdAsync(int concertId, CancellationToken cancellationToken)
    {
        var concert = await _concertRepository.GetEventByIdAsync(concertId, cancellationToken);
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

        var rejectionReasons = ValidateAvailableCapacityRules(existingConcert, request.NewCapacity);

        if (rejectionReasons.Any())
        {
            return Result<int>.Failure(rejectionReasons);
        }

        // Update the available capacity
        await _concertRepository.UpdateAvailableCapacityAsync(concertId, request.NewCapacity, cancellationToken).ConfigureAwait(false);
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
        var existingConcert = await _concertRepository.GetEventByIdAsync(concertId, cancellationToken);
        if (existingConcert == null)
        {
            return Result<Concert>.Failure(["Concert not found."]); // Fix: Wrap the string in an array to match IEnumerable<string>
        }

        var rejectionReasons = ValidateBusinessRules(entity);
        if (rejectionReasons.Any())
        {
            return Result<Concert>.Failure(rejectionReasons);
        }

        var updatedConcert = await _concertRepository.UpdateEventAsync(entity, cancellationToken);

        if (updatedConcert == null)
        {
            return Result<Concert>.Failure(new[] { "Concert not found." }); // Fix: Wrap the string in an array to match IEnumerable<string>
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
}
