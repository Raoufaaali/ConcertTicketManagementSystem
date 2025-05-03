using Concert_Ticket_Management_System.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcertTicketManagementSystem.Tests
{
    public static class Samples
    {
        public static Concert GetSampleConcert(Action<Concert>? customize = null)
        {
            var concert = new Concert
            {
                Id = 1,
                Name = "Sample Concert",
                TotalCapacity = 100,
                ConcertDate = DateTime.Now.AddDays(1),
                Description = "This is a sample concert for testing purposes."
            };
            customize?.Invoke(concert);
            return concert;
        }

        public static Reservation GetSampleReservation(Action<Reservation>? customize = null)
        {
            var reservation = new Reservation
            {
                Id = 1,
                ConcertId = 1,
                TicketType = TicketType.VIP,
                Quantity = 1
            };
            customize?.Invoke(reservation);
            return reservation;
        }
    }
}
