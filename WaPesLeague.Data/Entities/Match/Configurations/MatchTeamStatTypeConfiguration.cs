using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.Match.Configurations
{
    public class MatchTeamStatTypeConfiguration : IEntityTypeConfiguration<MatchTeamStatType>
    {
        public void Configure(EntityTypeBuilder<MatchTeamStatType> builder)
        {
            builder.Property(mtst => mtst.Code).HasMaxLength(20).IsRequired(true);
            builder.Property(mtst => mtst.Description).HasMaxLength(200).IsRequired(false);
            builder.Property(mtst => mtst.Order).IsRequired(true);

            builder.HasMany(mtst => mtst.MatchTeamStats)
                .WithOne(mts => mts.MatchTeamStatType)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
