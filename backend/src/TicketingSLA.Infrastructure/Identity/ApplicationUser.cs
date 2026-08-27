using Microsoft.AspNetCore.Identity;

namespace TicketingSLA.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public Guid TenantId { get; set; }
    public string DisplayName { get; set; } = null!;
}
