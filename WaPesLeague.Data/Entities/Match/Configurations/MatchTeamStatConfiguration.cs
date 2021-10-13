using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.Match.Configurations
{
    public class MatchTeamStatConfiguration : IEntityTypeConfiguration<MatchTeamStat>
    {
        public void Configure(EntityTypeBuilder<MatchTeamStat> builder)
        {
            builder.Property(mts => mts.Value).IsRequired(true);

            builder.HasOne(mts => mts.MatchTeam)
                .WithMany(mt => mt.MatchTeamStats)
                .HasForeignKey(mts => mts.MatchTeamId);

            builder.HasOne(mts => mts.MatchTeamStatType)
                .WithMany(mt => mt.MatchTeamStats)
                .HasForeignKey(mts => mts.MatchTeamStatTypeId);
        }
    }
}
