using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.Mix.Configurations
{
    public class MixChannelTeamTagConfiguration : IEntityTypeConfiguration<MixChannelTeamTag>
    {
        public void Configure(EntityTypeBuilder<MixChannelTeamTag> builder)
        {
            builder.Property(mt => mt.Tag).HasMaxLength(50).IsRequired(true);
            
            builder.HasOne(mctt => mctt.MixChannelTeam)
                .WithMany(mct => mct.Tags)
                .HasForeignKey(mctt => mctt.MixChannelTeamId);
        }
    }
}
