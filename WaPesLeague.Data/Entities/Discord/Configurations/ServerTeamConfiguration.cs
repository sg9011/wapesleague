using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.Discord.Configurations
{
    public class ServerTeamConfiguration : IEntityTypeConfiguration<ServerTeam>
    {
        public void Configure(EntityTypeBuilder<ServerTeam> builder)
        {
            builder.Property(st => st.Name).HasMaxLength(50).IsRequired(true);
            builder.Property(st => st.IsOpen).IsRequired(true);

            builder.HasOne(st => st.Server)
                .WithMany(s => s.DefaultTeams)
                .HasForeignKey(st => st.ServerId);

            builder.HasMany(st => st.Tags)
                .WithOne(f => f.Team)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
