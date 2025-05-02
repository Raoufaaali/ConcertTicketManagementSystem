using Concert_Ticket_Management_System.DTOs;
using FluentValidation;

namespace Concert_Ticket_Management_System.Shared.Validators;

public class ConcertDTOValidator : AbstractValidator<ConcertDTO>
{
    public ConcertDTOValidator()
    {
        RuleFor(x => x.ConcertDate)
            .NotEmpty()
            .WithMessage("Concert date is required.")
            .Must(date => date > DateTimeOffset.UtcNow)
            .WithMessage("Concert date must be in the future.");
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Concert name is required.")
            .MaximumLength(100)
            .WithMessage("Concert name must not exceed 100 characters.");
        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Description must not exceed 500 characters.");
        RuleFor(x => x.TotalCapacity)
            .GreaterThan(0)
            .WithMessage("Total capacity must be greater than zero.");
        RuleFor(x => x.TicketPricing)
            .NotEmpty()
            .WithMessage("Ticket pricing is required.")
            .Must(pricing => pricing.Values.All(price => price > 0))
            .WithMessage("All ticket prices must be greater than zero.");
    }
}
