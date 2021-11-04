using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.Mix.Configurations
{
    public class MixSessionConfiguration : IEntityTypeConfiguration<MixSession>
    {
        public void Configure(EntityTypeBuilder<MixSession> builder)
        {
            builder.Property(ms => ms.DateRegistrationOpening).IsRequired(true);
            builder.Property(ms => ms.DateStart).IsRequired(true);
            builder.Property(ms => ms.DateToClose).IsRequired(true);
            builder.Property(ms => ms.DateLastUpdated).IsRequired(false);
            builder.Property(ms => ms.DateCreated).IsRequired(false);
            builder.Property(ms => ms.DateStatsCalculated).IsRequired(false);
            builder.Property(ms => ms.CrashCount).IsRequired(true);
            builder.Property(ms => ms.MatchCount).IsRequired(true);
            builder.Property(ms => ms.DateClosed).IsRequired(false);
            builder.Property(ms => ms.Server).HasMaxLength(30).IsRequired(false);
            builder.Property(ms => ms.Password).HasMaxLength(20).IsRequired(false);
            builder.Property(ms => ms.GameRoomName).HasMaxLength(50).IsRequired(false);
            builder.Property(ms => ms.DateClosed).IsRequired(false);
            builder.Property(ms => ms.RoomOwnerId).IsRequired(false);

            builder.HasOne(ms => ms.MixChannel)
                .WithMany(mc => mc.MixSessions)
                .HasForeignKey(ms => ms.MixChannelId);

            builder.HasMany(ms => ms.MixTeams)
                .WithOne(mt => mt.MixSession)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ms => ms.RoomOwner)
                .WithMany(u => u.OwnerOfSessions)
                .HasForeignKey(ms => ms.RoomOwnerId);

            builder.HasMany(ms => ms.UserPositionStats)
                .WithOne(mups => mups.MixSession)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
