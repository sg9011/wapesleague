using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.Association.Configurations
{
    public class AssociationTeamPlayerConfiguration : IEntityTypeConfiguration<AssociationTeamPlayer>
    {
        public void Configure(EntityTypeBuilder<AssociationTeamPlayer> builder)
        {
            builder.Property(atp => atp.Start).IsRequired(true);
            builder.Property(atp => atp.End).IsRequired(false);
            builder.Property(atp => atp.TeamMemberType).HasConversion<string>().HasMaxLength(20).IsRequired(true);

            builder.HasOne(atp => atp.AssociationTeam)
                .WithMany(at => at.AssociationTeamPlayers)
                .HasForeignKey(atp => atp.AssociationTeamId); ;

            builder.HasOne(atp => atp.AssociationTenantPlayer)
                .WithMany(atp => atp.AssociationTeamPlayers)
                .HasForeignKey(atp => atp.AssociationTenantPlayerId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(atp => atp.MatchTeamPlayers)
                .WithOne(mtp => mtp.AssociationTeamPlayer)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
