using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.Mix.Configurations
{
    public class MixTeamRoleOpeningConfiguration : IEntityTypeConfiguration<MixTeamRoleOpening>
    {
        public void Configure(EntityTypeBuilder<MixTeamRoleOpening> builder)
        {
            builder.Property(mc => mc.Start).IsRequired(true);
            builder.Property(mc => mc.End).IsRequired(true);

            builder.HasOne(mx => mx.ServerRole)
                .WithMany(s => s.MixTeamRoleOpenings)
                .HasForeignKey(mx => mx.ServerRoleId);

            builder.HasOne(mx => mx.MixTeam)
                .WithMany(s => s.MixTeamRoleOpenings)
                .HasForeignKey(mx => mx.MixTeamId);
        }
    }
}
