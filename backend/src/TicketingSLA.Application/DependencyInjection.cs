using Microsoft.Extensions.DependencyInjection;
using TicketingSLA.Application.Services;

namespace TicketingSLA.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => { }, typeof(DependencyInjection).Assembly);
        services.AddScoped<TicketService>();
        services.AddScoped<SLAPolicyService>();
        services.AddScoped<DashboardService>();
        services.AddScoped<TenantService>();
        return services;
    }
}