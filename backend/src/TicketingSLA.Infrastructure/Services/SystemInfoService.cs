using TicketingSLA.Application.Interfaces;

namespace TicketingSLA.Infrastructure.Services;

public class SystemInfoService : ISystemInfoService
{
    public string GetStatus() => "Infrastructure layer is alive and wired via DI.";
}