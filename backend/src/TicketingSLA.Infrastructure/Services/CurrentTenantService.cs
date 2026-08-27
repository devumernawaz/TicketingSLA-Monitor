using Microsoft.AspNetCore.Http;
using TicketingSLA.Application.Interfaces;
using TicketingSLA.Infrastructure.Identity;

namespace TicketingSLA.Infrastructure.Services;

public class CurrentTenantService : ICurrentTenantService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentTenantService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid TenantId
    {
        get
        {
            var claim = _httpContextAccessor.HttpContext?.User.FindFirst(AppClaimTypes.TenantId)?.Value;

            if (string.IsNullOrEmpty(claim) || !Guid.TryParse(claim, out var tenantId))
                throw new InvalidOperationException("No authenticated tenant context is available.");

            return tenantId;
        }
    }
}