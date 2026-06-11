using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YMMO.Backend.Domain.Entities;

namespace YMMO.Backend.Infrastructure.Data.Configurations;

public class AgencyConfiguration : IEntityTypeConfiguration<Agency>
{
    public void Configure(EntityTypeBuilder<Agency> builder)
    {
        builder.HasKey(a => a.AgencyId);
        builder.Property(a => a.Name).IsRequired().HasMaxLength(150);
        builder.Property(a => a.Email).IsRequired().HasMaxLength(200);
        builder.Property(a => a.PhoneNumber).HasMaxLength(20);

        // Agency → Location (N:1)
        builder.HasOne(a => a.Location)
            .WithMany(l => l.Agencies)
            .HasForeignKey(a => a.LocationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}