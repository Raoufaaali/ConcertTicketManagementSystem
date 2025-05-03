using Concert_Ticket_Management_System.DataAccessLayer.Repos.ReservationRepository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shouldly;

namespace ConcertTicketManagementSystem.Tests.Repositories;

[TestClass]
public class ReservationRepositoryTests
{
    private IReservationRepository _service;

    [TestInitialize]
    public void Setup()
    {
        _service = new ServiceCollection()
            .AddSingleton<ReservationRepository>()
            .AddLogging()
            .BuildServiceProvider()
            .GetRequiredService<ReservationRepository>();
    }

    [TestMethod]
    public async Task AddReservationAsync_ShouldReturnSuccess_WhenValidReservation()
    {
        var reservation = Samples.GetSampleReservation();
        var concert = Samples.GetSampleConcert();
        reservation.SetReservationExpiry(DateTime.Now.AddDays(1));

        var result = await _service.AddReservationAsync(reservation, concert, CancellationToken.None);
        
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Data.ShouldBeEquivalentTo(reservation);
    }

    [TestMethod]
    public async Task AddReservationAsync_ShouldReturnFailure_WhenConcertIsFull()
    {
        // create a concert with 1 capacity, add a resevation, then try to add again. Should fail
        var reservation = Samples.GetSampleReservation();
        var concert = Samples.GetSampleConcert(concert => concert.TotalCapacity = 0);
        reservation.SetReservationExpiry(DateTime.Now.AddDays(1));
        
        var result = await _service.AddReservationAsync(reservation, concert, CancellationToken.None);
        
        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldNotBeNull();
        result.Errors.FirstOrDefault().ShouldBeEquivalentTo("Concert is fully booked.");
    }

    [TestMethod]
    public async Task GetTicketAvailability_ShouldRemoveExpiredReservationsAsync()
    {
        // We are adding 2 reservations, in a concert with 100 availability.
        // One reservation expired and the other did not.
        // When we ask for availability, we should get 99 instead of 98 (since expired reservations are continuously cleaned up )
        var concert = Samples.GetSampleConcert();
        var expiredReservation = Samples.GetSampleReservation(reservation => reservation.SetReservationExpiry(DateTime.Now.AddDays(-1)));
        var activeResevation = Samples.GetSampleReservation(reservation => reservation.SetReservationExpiry(DateTime.Now.AddDays(1)));
        
        // Order matters. Add the expired last (because it will be dequeued when the other reservation is insert because it would be expired)
        await _service.AddReservationAsync(activeResevation, concert, CancellationToken.None);
        await _service.AddReservationAsync(expiredReservation, concert, CancellationToken.None);

        
        var result = await _service.GetTicketAvailabilityAsync(concert, CancellationToken.None);

        result.ShouldBeEquivalentTo(99);
    }
}
