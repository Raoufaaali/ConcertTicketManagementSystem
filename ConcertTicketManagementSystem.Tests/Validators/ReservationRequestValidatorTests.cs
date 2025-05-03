using Concert_Ticket_Management_System.Domain.Entities;
using Concert_Ticket_Management_System.DTOs;
using Concert_Ticket_Management_System.Shared.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcertTicketManagementSystem.Tests.Validators
{
    [TestClass]
    public class ReservationRequestValidatorTests
    {
        [TestMethod]
        public void Validate_ValidRequest_ReturnsNoErrors()
        {
            // Arrange
            var validator = new ReservationRequestValidator();
            var validRequest = GetReservationRequest();
            validator.Validate(validRequest);
            // Act
            var result = validator.Validate(validRequest);
            // Assert
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_ZeroTicketQuantity_ReturnsErrors()
        {
            // Arrange
            var validator = new ReservationRequestValidator();
            var requestWithZeroQuantity = GetReservationRequest(x => x.Quantity = 0);
            // Act
            var result = validator.Validate(requestWithZeroQuantity);
            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual("Quantity of tickets must be greater than zero.", result.Errors[0].ErrorMessage);
        }


        public static ReservationRequest GetReservationRequest(Action<ReservationRequest>? customize = default)
        {
            var model = new ReservationRequest
            {
                ConcertId = 1,
                TicketType = TicketType.VIP,
                Quantity = 2
            };
            customize?.Invoke(model);
            return model;
        }
    }
}
