using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.Mix.Configurations
{
    public class MixTeamConfiguration : IEntityTypeConfiguration<MixTeam>
    {
        public void Configure(EntityTypeBuilder<MixTeam> builder)
        {
            builder.Property(mt => mt.Name).HasMaxLength(50).IsRequired(true);
            builder.Property(mt => mt.PositionsLocked).IsRequired(true);
            builder.Property(mt => mt.LockedTeamPlayerCount).IsRequired(false);

            builder.HasOne(mt => mt.MixSession)
                .WithMany(ms => ms.MixTeams)
                .HasForeignKey(mt => mt.MixSessionId);

            builder.HasMany(mt => mt.Formation)
                .WithOne(f => f.MixTeam)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
