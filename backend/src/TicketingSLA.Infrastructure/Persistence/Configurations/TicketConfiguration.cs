using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketingSLA.Domain.Entities;

namespace TicketingSLA.Infrastructure.Persistence.Configurations;

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.ToTable("Tickets");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Description)
            .HasMaxLength(2000);

        builder.Property(t => t.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(t => t.Priority)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(t => t.CreatedAt).IsRequired();
        builder.Property(t => t.SlaDeadline).IsRequired();
        builder.Property(t => t.BreachedAt);
        builder.Property(t => t.AssignedAgentId);

        // Declare the shadow property here, in the same place it's used —
        // instead of relying on OnModelCreating declaring it later.
        builder.Property<Guid>("TenantId").IsRequired();

        builder.HasIndex("TenantId", nameof(Ticket.Status), nameof(Ticket.SlaDeadline))
            .HasDatabaseName("IX_Tickets_TenantId_Status_SlaDeadline");
    }
}