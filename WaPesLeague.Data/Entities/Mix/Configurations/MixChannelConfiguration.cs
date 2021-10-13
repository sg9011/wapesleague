using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.Mix.Configurations
{
    public class MixChannelConfiguration : IEntityTypeConfiguration<MixChannel>
    {
        public void Configure(EntityTypeBuilder<MixChannel> builder)
        {
            builder.HasIndex(mc => mc.DiscordChannelId).HasDatabaseName("IX_MixChannel_DiscordChannelId");
            builder.Property(mc => mc.DiscordChannelId).HasMaxLength(100).IsRequired(true);
            builder.Property(mc => mc.ChannelName).HasMaxLength(100).IsRequired(true);
            builder.Property(mc => mc.Order).IsRequired(true);
            builder.Property(mc => mc.Enabled).IsRequired(true);

             builder.HasMany(mc => mc.MixSessions)
                .WithOne(ms => ms.MixChannel)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(mc => mc.MixChannelTeams)
                .WithOne(ta => ta.MixChannel)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(mc => mc.MixGroup)
                .WithMany(mx => mx.MixChannels)
                .HasForeignKey(mc => mc.MixGroupId);
        }
    }
}
