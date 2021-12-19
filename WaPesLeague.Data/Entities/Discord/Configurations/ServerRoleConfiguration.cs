using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.Discord.Configurations
{
    public class ServerRoleConfiguration : IEntityTypeConfiguration<ServerRole>
    {
        public void Configure(EntityTypeBuilder<ServerRole> builder)
        {
            builder.HasIndex(sr => sr.DiscordRoleId).HasDatabaseName("IX_Server_DiscordServerRoleIdId");
            builder.Property(sr => sr.DiscordRoleId).HasMaxLength(50).IsRequired(true);
            builder.Property(sr => sr.Name).HasMaxLength(50).IsRequired(true);
            builder.Property(sr => sr.Description).HasMaxLength(400).IsRequired(false);

            builder.HasOne(sr => sr.Server)
                .WithMany(s => s.ServerRoles)
                .HasForeignKey(sr => sr.ServerId);

            builder.HasMany(sr => sr.MixGroupRoleOpenings)
                .WithOne(s => s.ServerRole)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(sr => sr.MixTeamRoleOpenings)
                .WithOne(s => s.ServerRole)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(sr => sr.UserMemberServerRoles)
                .WithOne(umsr => umsr.ServerRole)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
