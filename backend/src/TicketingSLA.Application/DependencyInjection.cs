using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TicketingSLA.Application.DTOs.Auth;
using TicketingSLA.Application.DTOs.SLAPolicies;
using TicketingSLA.Application.DTOs.Tenants;
using TicketingSLA.Application.DTOs.Tickets;
using TicketingSLA.Application.Services;
using TicketingSLA.Application.Validators.Auth;
using TicketingSLA.Application.Validators.SLAPolicies;
using TicketingSLA.Application.Validators.Tenants;
using TicketingSLA.Application.Validators.Tickets;

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

        services.AddScoped<IValidator<CreateTicketRequest>, CreateTicketRequestValidator>();
        services.AddScoped<IValidator<UpdateTicketRequest>, UpdateTicketRequestValidator>();
        services.AddScoped<IValidator<AssignTicketRequest>, AssignTicketRequestValidator>();
        services.AddScoped<IValidator<CreateSLAPolicyRequest>, CreateSLAPolicyRequestValidator>();
        services.AddScoped<IValidator<UpdateSLAPolicyRequest>, UpdateSLAPolicyRequestValidator>();
        services.AddScoped<IValidator<RegisterRequest>, RegisterRequestValidator>();
        services.AddScoped<IValidator<LoginRequest>, LoginRequestValidator>();
        services.AddScoped<IValidator<CreateTenantRequest>, CreateTenantRequestValidator>();

        return services;
    }
}