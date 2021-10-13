using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.Association.Configurations
{
    public class AssociationTenantConfiguration : IEntityTypeConfiguration<AssociationTenant>
    {
        public void Configure(EntityTypeBuilder<AssociationTenant> builder)
        {
            builder.Property(at => at.Name).HasMaxLength(50).IsRequired(true);
            builder.Property(at => at.Code).HasMaxLength(10).IsRequired(true);
            builder.Property(at => at.Website).HasMaxLength(50).IsRequired(false);

            builder.HasMany(at => at.Associations)
                .WithOne(a => a.AssociationTenant)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(at => at.AssociationTenantPlayers)
                .WithOne(a => a.AssociationTenant)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
