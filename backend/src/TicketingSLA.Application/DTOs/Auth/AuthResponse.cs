using TicketingSLA.Domain.Enums;

namespace TicketingSLA.Application.DTOs.Auth;

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public Guid TenantId { get; set; }
}
