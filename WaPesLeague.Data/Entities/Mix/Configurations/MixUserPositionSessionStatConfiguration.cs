using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.Mix.Configurations
{
    public class MixUserPositionSessionStatConfiguration : IEntityTypeConfiguration<MixUserPositionSessionStat>
    {
        public void Configure(EntityTypeBuilder<MixUserPositionSessionStat> builder)
        {
            builder.Property(mups => mups.PlayTimeSeconds).IsRequired(true);

            builder.HasOne(mups => mups.User)
                .WithMany(u => u.PositionSessionStats)
                .HasForeignKey(mups => mups.UserId);

            builder.HasOne(mups => mups.MixSession)
                .WithMany(ms => ms.UserPositionStats)
                .HasForeignKey(mups => mups.MixSessionId);

            builder.HasOne(mups => mups.Position)
                .WithMany(p => p.UserSessionStats)
                .HasForeignKey(mups => mups.PositionId);
        }
    }
}
