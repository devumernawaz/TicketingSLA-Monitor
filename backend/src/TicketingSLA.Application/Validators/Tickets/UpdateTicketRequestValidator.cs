using FluentValidation;
using TicketingSLA.Application.DTOs.Tickets;

namespace TicketingSLA.Application.Validators.Tickets;

public class UpdateTicketRequestValidator : AbstractValidator<UpdateTicketRequest>
{
    public UpdateTicketRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(2000);
    }
}
