namespace Concert_Ticket_Management_System.Shared.Validators;

using FluentValidation;

public sealed class ConcertIdValidator : AbstractValidator<int>
{
    public ConcertIdValidator()
    {
        RuleFor(concertId => concertId)
            .GreaterThan(0)
            .WithMessage("Concert ID must be greater than zero.");
    }
}
