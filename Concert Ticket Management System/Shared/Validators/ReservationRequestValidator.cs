using Concert_Ticket_Management_System.Domain.Entities;
using Concert_Ticket_Management_System.DTOs;
using FluentValidation;

namespace Concert_Ticket_Management_System.Shared.Validators;

public class ReservationRequestValidator : AbstractValidator<ReservationRequest>
{
    public ReservationRequestValidator()
    {
        RuleFor(x => x.ConcertId)
            .NotEmpty()
            .WithMessage("Concert ID is required.");
        RuleFor(x => x.ConcertId)
            .GreaterThan(0)
            .WithMessage("Concert ID must be greater than zero.");
        RuleFor(x => x.TicketType)
            .IsInEnum()
            .WithMessage("Invalid ticket type.")
        .NotEqual(TicketType.Unknown)
            .WithMessage("TicketType cannot be Unknown."); // Exclude Unknown
        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity of tickets must be greater than zero.");
    }
}

