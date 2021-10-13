using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.Mix.Configurations
{
    public class MixChannelTeamPositionConfiguration : IEntityTypeConfiguration<MixChannelTeamPosition>
    {
        public void Configure(EntityTypeBuilder<MixChannelTeamPosition> builder)
        {
            builder.HasOne(mcp => mcp.MixChannelTeam)
                .WithMany(mc => mc.DefaultFormation)
                .HasForeignKey(mcp => mcp.MixChannelTeamId);

            builder.HasOne(mcp => mcp.Position)
                .WithMany(mc => mc.MixChannelPositions)
                .HasForeignKey(mcp => mcp.PositionId);
        }
    }
}
