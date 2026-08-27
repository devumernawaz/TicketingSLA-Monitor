using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using TicketingSLA.Application.Interfaces;

namespace TicketingSLA.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public Guid UserId
    {
        get
        {
            var claim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(claim) || !Guid.TryParse(claim, out var userId))
                throw new InvalidOperationException("No authenticated user context is available.");

            return userId;
        }
    }

    public string Role =>
        User?.FindFirst(ClaimTypes.Role)?.Value
        ?? throw new InvalidOperationException("No authenticated user role is available.");

    public string Email =>
        User?.FindFirst(ClaimTypes.Email)?.Value
        ?? throw new InvalidOperationException("No authenticated user email is available.");
}
