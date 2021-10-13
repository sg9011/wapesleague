using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.Mix.Configurations
{
    public class MixPositionConfiguration : IEntityTypeConfiguration<MixPosition>
    {
        public void Configure(EntityTypeBuilder<MixPosition> builder)
        {
            builder.Property(mp => mp.DateStart).IsRequired(true);
            builder.Property(mp => mp.DateEnd).IsRequired(false);

            builder.HasOne(mp => mp.MixTeam)
                .WithMany(mt => mt.Formation)
                .HasForeignKey(mp => mp.MixTeamId);

            builder.HasOne(mp => mp.Position)
                .WithMany(p => p.MixPositions)
                .HasForeignKey(mp => mp.PositionId);
        }
    }
}
