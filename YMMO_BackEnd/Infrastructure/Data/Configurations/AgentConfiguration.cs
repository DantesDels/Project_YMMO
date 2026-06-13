using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YMMO.Backend.Domain.Entities;

namespace YMMO.Backend.Infrastructure.Data.Configurations;

public class AgentConfiguration : IEntityTypeConfiguration<Agent>
{
    public void Configure(EntityTypeBuilder<Agent> builder)
    {
        // Agent → Agency (N:1)
        builder.HasOne(ag => ag.Agency)
            .WithMany(a => a.Agents)
            .HasForeignKey(ag => ag.AgencyId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Metadata.FindNavigation(nameof(Agent.Properties))
            ?.SetPropertyAccessMode(PropertyAccessMode.Field);
        
        // Property (1:N)
        builder.HasMany(a => a.Properties)
            .WithOne(p => p.Agent)
            .HasForeignKey(p => p.AgentId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}