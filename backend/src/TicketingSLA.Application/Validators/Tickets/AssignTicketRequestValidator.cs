using FluentValidation;
using TicketingSLA.Application.DTOs.Tickets;

namespace TicketingSLA.Application.Validators.Tickets;

public class AssignTicketRequestValidator : AbstractValidator<AssignTicketRequest>
{
    public AssignTicketRequestValidator()
    {
        RuleFor(x => x.AgentId).NotEmpty();
    }
}
