using Concert_Ticket_Management_System.Domain.Entities;
using Concert_Ticket_Management_System.Services.ConcertServices;
using Concert_Ticket_Management_System.Shared;
using Concert_Ticket_Management_System.Shared.Validators;
using System;
using System.Collections.Concurrent;

namespace Concert_Ticket_Management_System.DataAccessLayer.Repos.ReservationRepository;

public class ReservationRepository : IReservationRepository
{
    private readonly ConcurrentDictionary<int, ConcurrentDictionary<int, Reservation>> _db = new(); // reservation DB
    #region
 /*
 * I need additional something to manage the timed reservations
 * Before I add the reservations, I need to do some house keeping
 * This include checking current reservations and removing the expired ones
 * I don't want to walk the entire store to do this
 * So I need the clean up to be done in O(log n) time. 
 * A heap is a good candidate for this. I will try to use a dictionary where the key is the concert Id and the value is a min heap (special type of tree) with priority item lookup in O(log n) time
 * 
 */
    #endregion

    private readonly ILogger<ReservationRepository> _logger;
    private readonly ConcurrentDictionary<int, PriorityQueue<Reservation, DateTimeOffset>> _stateTracker = new();

    private int _currentId = 0;

    public ReservationRepository(ILogger<ReservationRepository> logger)
    {
        _logger = logger;
    }

    public Task<Result<Reservation>> AddReservationAsync(Reservation reservation, Concert concert, CancellationToken cancellationToken)
    {
        if (reservation == null)
        {
            return Task.FromResult(Result<Reservation>.Failure(new[] { "Reservation cannot be null." }));
        }

        if (concert == null)
        {
            return Task.FromResult(Result<Reservation>.Failure(new[] { "Concert cannot be null." }));
        }

        // Step 1: Dequeue expired items
        DequeueExpiredItems(concert.Id);

        // Step 2: Calculate current capacity
        var currentCapacity = CalculateCapacity(concert);

        // Step 3: Check if we can insert
        if (!CanInsert(currentCapacity, concert.TotalCapacity))
        {
            return Task.FromResult(Result<Reservation>.Failure(new[] { "Concert is fully booked." }));
        }

        // Step 4: Insert the reservation
        var newReservation = InsertReservation(reservation, concert.Id);

        return Task.FromResult(Result<Reservation>.Success(newReservation));
    }

    public Task<Reservation>? UpdateReservationAsync(Reservation reservation, CancellationToken cancellationToken)
    {
        if (_db.TryGetValue(reservation.ConcertId, out var reservations))
        {
            if (reservations.ContainsKey(reservation.Id))
            {
                reservations[reservation.Id] = reservation;
                _logger.LogInformation($"Reservation with ID {reservation.Id} updated in the database.");
                return Task.FromResult(reservation);
            }
        }

        _logger.LogWarning($"Reservation with ID {reservation.Id} not found in the database.");
        return Task.FromResult<Reservation>(null);
    }

    // Method 1: Dequeue expired items from the priority queue
    private void DequeueExpiredItems(int concertId)
    {
        if (_stateTracker.TryGetValue(concertId, out var minHeap))
        {
            while (minHeap.Count > 0 && minHeap.Peek().ReservationExpiry < DateTimeOffset.Now)
            {
                var expiredReservation = minHeap.Dequeue();
                if (_db.TryGetValue(concertId, out var reservations))
                {
                    if (expiredReservation.ReservationStatus == ReservationStatus.Canceled ||
                        expiredReservation.ReservationStatus == ReservationStatus.Pending)
                    {
                        reservations.TryRemove(expiredReservation.Id, out _);
                    }
                }
            }
            var x = minHeap.Count;
        }
    }

    // Method 2: Calculate capacity by checking how many reservations exist for a given concert ID
    private int CalculateCapacity(Concert concert)
    {
        if (_db.TryGetValue(concert.Id, out var reservations))
        {
            return reservations.Count;
        }
        return 0; // No reservations exist, so the current capacity is 0
    }

    // Method 3: Check if we can insert a new reservation
    private bool CanInsert(int currentCapacity, int totalCapacity)
    {
        return currentCapacity < totalCapacity;
    }

    // Method 4: Insert the reservation into the dictionary and priority queue
    private Reservation InsertReservation(Reservation reservation, int concertId)
    {
        // Auto-increment the reservation ID in a thread-safe manner
        var newId = Interlocked.Increment(ref _currentId);
        reservation.Id = newId;

        // Ensure the nested dictionary exists for the concert ID
        var concertReservations = _db.GetOrAdd(concertId, _ => new ConcurrentDictionary<int, Reservation>());

        // Add the reservation to the nested dictionary
        if (!concertReservations.TryAdd(newId, reservation))
        {
            throw new InvalidOperationException($"Failed to add reservation with ID {newId}.");
        }

        // Add the reservation to the priority queue
        var minHeap = _stateTracker.GetOrAdd(concertId, _ => new PriorityQueue<Reservation, DateTimeOffset>());
        lock (minHeap) // Ensure thread safety when modifying the PriorityQueue
        {
            minHeap.Enqueue(reservation, reservation.ReservationExpiry);
        }

        _logger.LogInformation($"Reservation with ID {newId} added to the database and state tracker.");
        return reservation;
    }

    public async Task<Reservation?> GetReservationAsync(int reservationId, int concertId, CancellationToken cancellationToken)
    {
        // Clean up expired reservations for all concerts
        DequeueExpiredItems(concertId);

        // Look up the reservation in the database
        if (_db.TryGetValue(concertId, out var reservations))
        {
            if (reservations.TryGetValue(reservationId, out var reservation))
            {
                return reservation; // Return the reservation if found
            }
        }

        _logger.LogWarning($"Reservation with ID {reservationId} not found.");
        return null;
    }

    public Task<bool> CancelReservationAsync(int reservationId, int concertId, CancellationToken cancellationToken)
    {
        // Clean up expired reservations for the given concert
        DequeueExpiredItems(concertId);

        if (_db.TryGetValue(concertId, out var reservations))
        {
            if (reservations.TryRemove(reservationId, out _))
            {
                _logger.LogInformation("Reservation with ID {ReservationId} removed from the database for concert ID {ConcertId}.", reservationId, concertId);
                return Task.FromResult<bool>(true);
            }
        }

        _logger.LogWarning("Reservation with ID {ReservationId} not found for concert ID {ConcertId}.", reservationId, concertId);
        return Task.FromResult<bool>(false);
    }

    public Task<bool> ConfirmReservationAsync(Reservation reservation, int concertId, CancellationToken cancellationToken)
    {
        // Clean up expired reservations for the given concert
        DequeueExpiredItems(concertId);

        if (_db.TryGetValue(concertId, out var reservations))
        {
            if (reservations.TryGetValue(reservation.Id, out var existingReservation))
            {
                reservations[reservation.Id] = existingReservation;

                _logger.LogInformation($"Reservation with ID {reservation.Id} confirmed successfully.", reservation.Id);
                return Task.FromResult<bool>(true);
            }
        }

        _logger.LogWarning($"Reservation with ID {reservation.Id} not found for concert ID {concertId}.", reservation.Id, concertId);
        return Task.FromResult<bool>(false);
    }
}
