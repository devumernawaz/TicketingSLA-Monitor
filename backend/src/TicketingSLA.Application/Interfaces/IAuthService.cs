using TicketingSLA.Application.DTOs.Auth;
using TicketingSLA.Shared.Common;

namespace TicketingSLA.Application.Interfaces;

public interface IAuthService
{
    Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request);
    Task<Result<AuthResponse>> LoginAsync(LoginRequest request);
}
