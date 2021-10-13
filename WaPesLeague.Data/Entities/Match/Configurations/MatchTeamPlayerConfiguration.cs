using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.Match.Configurations
{
    public class MatchTeamPlayerConfiguration : IEntityTypeConfiguration<MatchTeamPlayer>
    {
        public void Configure(EntityTypeBuilder<MatchTeamPlayer> builder)
        {
            builder.Property(mtp => mtp.PlayerName).HasMaxLength(100).IsRequired(true);
            builder.Property(mtp => mtp.JerseyNumber).IsRequired(false);

            builder.HasOne(mtp => mtp.MatchTeam)
                .WithMany(mt => mt.MatchTeamPlayers)
                .HasForeignKey(mtp => mtp.MatchTeamId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(mtp => mtp.Position)
                .WithMany(p => p.MatchTeamPlayers)
                .HasForeignKey(mtp => mtp.PositionId);

            builder.HasOne(mtp => mtp.AssociationTeamPlayer)
                .WithMany(atp => atp.MatchTeamPlayers)
                .HasForeignKey(mtp => mtp.AssociationTeamPlayerId)
                .IsRequired(false);

            builder.HasMany(mtp => mtp.MatchTeamPlayerStats)
                .WithOne(mtps => mtps.MatchTeamPlayer)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(mtp => mtp.MatchTeamPlayerEvents)
                .WithOne(mtpe => mtpe.MatchTeamPlayer)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
