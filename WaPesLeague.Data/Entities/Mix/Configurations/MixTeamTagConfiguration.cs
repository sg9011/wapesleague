using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.Mix.Configurations
{
    public class MixTeamTagConfiguration : IEntityTypeConfiguration<MixTeamTag>
    {
        public void Configure(EntityTypeBuilder<MixTeamTag> builder)
        {
            builder.Property(mtt => mtt.Tag).HasMaxLength(50).IsRequired(true);

            builder.HasOne(mtt => mtt.MixTeam)
                .WithMany(mt => mt.Tags)
                .HasForeignKey(mtt => mtt.MixTeamId);
        }
    }
}
