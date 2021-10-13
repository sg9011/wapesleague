using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.Match.Configurations
{
    public class MatchTeamConfiguration : IEntityTypeConfiguration<MatchTeam>
    {
        public void Configure(EntityTypeBuilder<MatchTeam> builder)
        {
            builder.Property(mt => mt.Goals).IsRequired(false);
            builder.Property(mt => mt.DateConfirmed).IsRequired(false);

            builder.HasOne(mt => mt.Match)
                .WithMany(m => m.MatchTeams)
                .HasForeignKey(mt => mt.MatchId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(mt => mt.Team)
                .WithMany(t => t.MatchTeams)
                .HasForeignKey(mt => mt.MatchTeamId);

            builder.HasOne(mt => mt.ConfirmedBy)
                .WithMany(u => u.MatchTeamsConfirmedByUser)
                .HasForeignKey(mt => mt.ConfirmedById)
                .IsRequired(false);

            builder.HasMany(mt => mt.MatchTeamPlayers)
                .WithOne(mtp => mtp.MatchTeam)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(mt => mt.MatchTeamStats)
                .WithOne(mts => mts.MatchTeam)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
