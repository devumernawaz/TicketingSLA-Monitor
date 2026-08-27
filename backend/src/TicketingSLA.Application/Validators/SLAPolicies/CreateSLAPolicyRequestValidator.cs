using FluentValidation;
using TicketingSLA.Application.DTOs.SLAPolicies;

namespace TicketingSLA.Application.Validators.SLAPolicies;

public class CreateSLAPolicyRequestValidator : AbstractValidator<CreateSLAPolicyRequest>
{
    public CreateSLAPolicyRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Priority).IsInEnum();
        RuleFor(x => x.ResponseTimeHours).GreaterThan(0);
    }
}
