using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.Association.Configurations
{
    public class AssociationTenantPlayerConfiguration : IEntityTypeConfiguration<AssociationTenantPlayer>
    {
        public void Configure(EntityTypeBuilder<AssociationTenantPlayer> builder)
        {
            builder.Property(atp => atp.DateCreated).IsRequired(true);
            builder.Property(atp => atp.Name).HasMaxLength(100).IsRequired(true);

            builder.HasOne(atp => atp.User)
                .WithMany(u => u.AssociationTenantPlayers)
                .HasForeignKey(atp => atp.UserId);

            builder.HasOne(atp => atp.AssociationTenant)
                .WithMany(at => at.AssociationTenantPlayers)
                .HasForeignKey(atp => atp.AssociationTenantId);

            builder.HasMany(atp => atp.AssociationTeamPlayers)
                .WithOne(tp => tp.AssociationTenantPlayer)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
