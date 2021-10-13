using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.Mix.Configurations
{
    public class MixChannelTeamConfiguration : IEntityTypeConfiguration<MixChannelTeam>
    {
        public void Configure(EntityTypeBuilder<MixChannelTeam> builder)
        {
            builder.Property(mct => mct.MixChannelTeamName).HasMaxLength(50).IsRequired(true);
            builder.Property(mct => mct.IsOpen).IsRequired(true);


            builder.HasMany(mct => mct.DefaultFormation)
                .WithOne(mcp => mcp.MixChannelTeam)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(mct => mct.DefaultFormation)
                .WithOne(mcp => mcp.MixChannelTeam)
                .OnDelete(DeleteBehavior.Cascade);


            builder.HasOne(mct => mct.MixChannel)
                .WithMany(mc => mc.MixChannelTeams)
                .HasForeignKey(mct => mct.MixChannelId);
        }
    }
}
