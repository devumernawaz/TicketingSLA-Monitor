using Microsoft.EntityFrameworkCore;
using TicketingSLA.Application.Interfaces;
using TicketingSLA.Domain.Entities;

namespace TicketingSLA.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    private readonly ICurrentTenantService _currentTenantService;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentTenantService currentTenantService) : base(options)
    {
        _currentTenantService = currentTenantService;
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<SLAPolicy> SLAPolicies => Set<SLAPolicy>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<SLADashboardSummaryEntity> SLADashboardSummaries => Set<SLADashboardSummaryEntity>();
    public DbSet<SLADailyTrendEntity> SLADailyTrends => Set<SLADailyTrendEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        modelBuilder.Entity<SLAPolicy>().Property<Guid>("TenantId").IsRequired();

        modelBuilder.Entity<Ticket>()
            .HasQueryFilter(t => EF.Property<Guid>(t, "TenantId") == _currentTenantService.TenantId);

        modelBuilder.Entity<SLAPolicy>()
            .HasQueryFilter(p => EF.Property<Guid>(p, "TenantId") == _currentTenantService.TenantId);

        modelBuilder.Entity<SLADashboardSummaryEntity>().HasNoKey();
        modelBuilder.Entity<SLADailyTrendEntity>().HasNoKey();
    }

    public override int SaveChanges()
    {
        SetTenantId();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetTenantId();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void SetTenantId()
    {
        foreach (var entry in ChangeTracker.Entries()
                     .Where(e => e.State == EntityState.Added &&
                                 (e.Entity is Ticket || e.Entity is SLAPolicy)))
        {
            entry.Property("TenantId").CurrentValue = _currentTenantService.TenantId;
        }
    }
}