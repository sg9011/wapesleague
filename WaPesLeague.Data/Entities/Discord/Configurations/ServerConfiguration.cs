using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaPesLeague.Constants;
using WaPesLeague.Data.Entities.Discord;

namespace WaPesLeague.Data.Entities.User.Configurations
{
    public class ServerConfiguration : IEntityTypeConfiguration<Server>
    {
        public void Configure(EntityTypeBuilder<Server> builder)
        {
            builder.HasIndex(s => s.DiscordServerId).HasDatabaseName("IX_Server_DiscordServerId");
            builder.Property(s => s.DiscordServerId).HasMaxLength(50).IsRequired(true);
            builder.Property(s => s.ServerName).HasMaxLength(200).IsRequired(false);
            builder.Property(s => s.IsActive).IsRequired(true);

            builder.Property(s => s.DefaultSessionRecurringWithAClosedTeam).IsRequired(true);
            builder.Property(s => s.DefaultSessionRecurringWithAllOpen).IsRequired(true);
            builder.Property(s => s.DefaultAutoCreateExtraSessionsWhenAllTeamsOpen).IsRequired(true);
            builder.Property(s => s.DefaultAutoCreateExtraSessionsWithAClosedTeam).IsRequired(true);
            builder.Property(s => s.DefaultStartTime).HasPrecision(18, 4).IsRequired(true);
            builder.Property(s => s.DefaultHoursToOpenRegistrationBeforeStart).HasPrecision(18, 4).IsRequired(true);
            builder.Property(s => s.DefaultSessionDuration).HasPrecision(18, 4).IsRequired(true);

            builder.Property(s => s.DefaultSessionExtraInfo).HasMaxLength(500).IsRequired(false);
            builder.Property(s => s.DefaultSessionPassword).HasMaxLength(20).IsRequired(false);

            builder.Property(s => s.TimeZoneName).HasMaxLength(100).IsRequired(true);
            builder.Property(s => s.Language).HasMaxLength(5).HasDefaultValue(Bot.SupportedLanguages.English).IsRequired(true);

            builder.Property(s => s.AllowActiveSwapCommand).IsRequired(true);
            builder.Property(s => s.AllowInactiveSwapCommand).IsRequired(true);

            builder.HasMany(s => s.Members)
                .WithOne(sm => sm.Server)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(s => s.MixGroups)
                .WithOne(mg => mg.Server)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(s => s.ServerFormations)
                .WithOne(sf => sf.Server)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(s => s.DefaultTeams)
                .WithOne(t => t.Server)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(s => s.ServerRoles)
                .WithOne(sr => sr.Server)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(s => s.ServerEvents)
                .WithOne(sr => sr.Server)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(s => s.ButtonGroups)
                .WithOne(bg => bg.Server)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
