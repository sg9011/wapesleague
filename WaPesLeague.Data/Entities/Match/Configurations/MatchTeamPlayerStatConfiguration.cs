using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.Match.Configurations
{
    public class MatchTeamPlayerStatConfiguration : IEntityTypeConfiguration<MatchTeamPlayerStat>
    {
        public void Configure(EntityTypeBuilder<MatchTeamPlayerStat> builder)
        {
            builder.Property(mtps => mtps.Value).IsRequired(true);

            builder.HasOne(mtps => mtps.MatchTeamPlayer)
                .WithMany(mtp => mtp.MatchTeamPlayerStats)
                .HasForeignKey(mtpe => mtpe.MatchTeamPlayerId);

            builder.HasOne(mtps => mtps.MatchPlayerStatType)
                .WithMany(mtpst => mtpst.MatchTeamPlayerStats)
                .HasForeignKey(mtps => mtps.MatchPlayerStatTypeId);
        }
    }
}
