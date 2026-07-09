using Microsoft.AspNetCore.Http;
using TicketingSLA.Application.Interfaces;

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
            var header = _httpContextAccessor.HttpContext?.Request.Headers["X-Tenant-Id"].FirstOrDefault();

            if (string.IsNullOrEmpty(header) || !Guid.TryParse(header, out var parsedId))
                throw new InvalidOperationException("X-Tenant-Id header is missing or invalid.");

            return parsedId;
        }
    }
}