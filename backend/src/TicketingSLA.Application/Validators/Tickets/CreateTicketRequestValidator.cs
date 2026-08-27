using FluentValidation;
using TicketingSLA.Application.DTOs.Tickets;

namespace TicketingSLA.Application.Validators.Tickets;

public class CreateTicketRequestValidator : AbstractValidator<CreateTicketRequest>
{
    public CreateTicketRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(2000);
        RuleFor(x => x.Priority).IsInEnum();
    }
}
