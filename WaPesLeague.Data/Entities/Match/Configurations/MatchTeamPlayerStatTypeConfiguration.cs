using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.Match.Configurations
{
    public class MatchTeamPlayerStatTypeConfiguration : IEntityTypeConfiguration<MatchTeamPlayerStatType>
    {
        public void Configure(EntityTypeBuilder<MatchTeamPlayerStatType> builder)
        {
            builder.Property(mtp => mtp.Code).HasMaxLength(20).IsRequired(true);
            builder.Property(mtp => mtp.Description).HasMaxLength(200).IsRequired(false);
            builder.Property(mtp => mtp.Order).IsRequired(true);

            builder.HasMany(mtpst => mtpst.MatchTeamPlayerStats)
                .WithOne(mtps => mtps.MatchPlayerStatType)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
