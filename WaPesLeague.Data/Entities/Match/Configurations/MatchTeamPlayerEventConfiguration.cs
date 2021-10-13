using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.Match.Configurations
{
    public class MatchTeamPlayerEventConfiguration : IEntityTypeConfiguration<MatchTeamPlayerEvent>
    {
        public void Configure(EntityTypeBuilder<MatchTeamPlayerEvent> builder)
        {
            builder.Property(mtpe => mtpe.Minute).IsRequired(false);
            builder.Property(mtpe => mtpe.Period).HasConversion<string>().HasMaxLength(30).IsRequired(false);
            builder.Property(mtpe => mtpe.Event).HasConversion<string>().HasMaxLength(30).IsRequired(true);

            builder.HasOne(mtpe => mtpe.MatchTeamPlayer)
                .WithMany(mtp => mtp.MatchTeamPlayerEvents)
                .HasForeignKey(mtpe => mtpe.MatchTeamPlayerId);
            
        }
    }
}
