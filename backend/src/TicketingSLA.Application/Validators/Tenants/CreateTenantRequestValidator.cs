using FluentValidation;
using TicketingSLA.Application.DTOs.Tenants;

namespace TicketingSLA.Application.Validators.Tenants;

public class CreateTenantRequestValidator : AbstractValidator<CreateTenantRequest>
{
    public CreateTenantRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}
