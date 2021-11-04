using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.Discord.Configurations
{
    public class ServerRoleConfiguration : IEntityTypeConfiguration<ServerRole>
    {
        public void Configure(EntityTypeBuilder<ServerRole> builder)
        {
            builder.HasIndex(s => s.DiscordRoleId).HasDatabaseName("IX_Server_DiscordServerRoleIdId");
            builder.Property(s => s.DiscordRoleId).HasMaxLength(50).IsRequired(true);
            builder.Property(st => st.Name).HasMaxLength(50).IsRequired(true);
            builder.Property(st => st.Description).HasMaxLength(400).IsRequired(false);

            builder.HasOne(st => st.Server)
                .WithMany(s => s.ServerRoles)
                .HasForeignKey(st => st.ServerId);

            builder.HasMany(st => st.MixGroupRoleOpenings)
                .WithOne(s => s.ServerRole)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(st => st.MixTeamRoleOpenings)
                .WithOne(s => s.ServerRole)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
