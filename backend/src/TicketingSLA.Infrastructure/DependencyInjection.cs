using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TicketingSLA.Application.Interfaces;
using TicketingSLA.Infrastructure.Alerts;
using TicketingSLA.Infrastructure.BackgroundServices;
using TicketingSLA.Infrastructure.Persistence;
using TicketingSLA.Infrastructure.Repositories;
using TicketingSLA.Infrastructure.Services;


namespace TicketingSLA.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ISystemInfoService, SystemInfoService>();

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentTenantService, CurrentTenantService>();
        services.AddScoped<ITicketRepository, TicketRepository>();
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<ISLAPolicyRepository, SLAPolicyRepository>();
        services.AddHostedService<SLABreachMonitorService>();
        services.AddScoped<IAlertService, MockAlertService>();
        services.AddScoped<IDashboardRepository, DashboardRepository>();

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        return services;
    }
}