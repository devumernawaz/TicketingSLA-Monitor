using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TicketingSLA.Application.Interfaces;
using TicketingSLA.Domain.Enums;
using TicketingSLA.Infrastructure.Persistence;

namespace TicketingSLA.Infrastructure.BackgroundServices;

public class SLABreachMonitorService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SLABreachMonitorService> _logger;
    private static readonly TimeSpan ScanInterval = TimeSpan.FromSeconds(60);
    private const string LockResourceName = "SLABreachMonitor_Scan";

    public SLABreachMonitorService(IServiceScopeFactory scopeFactory, ILogger<SLABreachMonitorService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunScanWithDistributedLockAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SLA breach scan failed.");
            }

            await Task.Delay(ScanInterval, stoppingToken);
        }
    }

    private async Task RunScanWithDistributedLockAsync(CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var alertService = scope.ServiceProvider.GetRequiredService<IAlertService>();

        await using var connection = new SqlConnection(context.Database.GetConnectionString());
        await connection.OpenAsync(stoppingToken);

        var acquired = await TryAcquireLockAsync(connection, stoppingToken);
        if (!acquired)
        {
            _logger.LogInformation("SLA scan skipped — another instance holds the lock.");
            return;
        }

        try
        {
            await ScanForBreachesAsync(context, alertService, stoppingToken);
        }
        finally
        {
            await ReleaseLockAsync(connection, stoppingToken);
        }
    }

    private async Task<bool> TryAcquireLockAsync(SqlConnection connection, CancellationToken stoppingToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = "sp_getapplock";
        command.CommandType = System.Data.CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@Resource", LockResourceName);
        command.Parameters.AddWithValue("@LockMode", "Exclusive");
        command.Parameters.AddWithValue("@LockOwner", "Session");
        command.Parameters.AddWithValue("@LockTimeout", 0);

        var returnParam = command.Parameters.Add("@ReturnValue", System.Data.SqlDbType.Int);
        returnParam.Direction = System.Data.ParameterDirection.ReturnValue;

        await command.ExecuteNonQueryAsync(stoppingToken);

        var result = (int)returnParam.Value!;
        return result >= 0;
    }

    private async Task ReleaseLockAsync(SqlConnection connection, CancellationToken stoppingToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = "sp_releaseapplock";
        command.CommandType = System.Data.CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@Resource", LockResourceName);
        command.Parameters.AddWithValue("@LockOwner", "Session");

        await command.ExecuteNonQueryAsync(stoppingToken);
    }

    private async Task ScanForBreachesAsync(ApplicationDbContext context, IAlertService alertService, CancellationToken stoppingToken)
    {
        var now = DateTime.UtcNow;

        var candidateTickets = context.Tickets
            .IgnoreQueryFilters()
            .Where(t => t.Status != TicketStatus.Closed && t.BreachedAt == null)
            .ToList();

        var breachedCount = 0;
        foreach (var ticket in candidateTickets)
        {
            if (ticket.IsBreached(now))
            {
                ticket.MarkBreached(now);
                breachedCount++;

                var tenantId = (Guid)context.Entry(ticket).Property("TenantId").CurrentValue!;
                await alertService.SendBreachAlertAsync(ticket, tenantId);
            }
        }

        if (breachedCount > 0)
        {
            await context.SaveChangesAsync(stoppingToken);
            _logger.LogInformation("SLA scan: marked {Count} ticket(s) as breached.", breachedCount);
        }
        else
        {
            _logger.LogInformation("SLA scan: no new breaches detected.");
        }
    }
}