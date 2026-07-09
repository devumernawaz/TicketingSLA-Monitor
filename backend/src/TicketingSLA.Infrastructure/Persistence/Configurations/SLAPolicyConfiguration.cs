using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketingSLA.Domain.Entities;

namespace TicketingSLA.Infrastructure.Persistence.Configurations;

public class SLAPolicyConfiguration : IEntityTypeConfiguration<SLAPolicy>
{
    public void Configure(EntityTypeBuilder<SLAPolicy> builder)
    {
        builder.ToTable("SLAPolicies");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Priority)
            .IsRequired()
            .HasConversion<string>()   // store enum as readable text, not a magic number
            .HasMaxLength(20);

        builder.Property(p => p.ResponseTimeHours)
            .IsRequired();


    }
}