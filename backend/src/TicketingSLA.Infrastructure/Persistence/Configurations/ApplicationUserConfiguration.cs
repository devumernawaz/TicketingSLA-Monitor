using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketingSLA.Infrastructure.Identity;

namespace TicketingSLA.Infrastructure.Persistence.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(u => u.TenantId).IsRequired();
        builder.Property(u => u.DisplayName).IsRequired().HasMaxLength(200);
        builder.HasIndex(u => u.TenantId);
    }
}
