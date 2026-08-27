using FluentValidation;
using TicketingSLA.Application.DTOs.Auth;

namespace TicketingSLA.Application.Validators.Auth;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.Role).IsInEnum();
    }
}
