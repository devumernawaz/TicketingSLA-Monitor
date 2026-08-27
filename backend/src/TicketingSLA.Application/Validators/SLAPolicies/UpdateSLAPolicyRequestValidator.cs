using FluentValidation;
using TicketingSLA.Application.DTOs.SLAPolicies;

namespace TicketingSLA.Application.Validators.SLAPolicies;

public class UpdateSLAPolicyRequestValidator : AbstractValidator<UpdateSLAPolicyRequest>
{
    public UpdateSLAPolicyRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ResponseTimeHours).GreaterThan(0);
    }
}
