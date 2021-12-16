using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaPesLeague.Data.Entities.User;

namespace WaPesLeague.Data.Entities.Discord.Configurations
{
    public class UserMemberConfiguration : IEntityTypeConfiguration<UserMember>
    {
        public void Configure(EntityTypeBuilder<UserMember> builder)
        {
            builder.Property(um => um.DiscordNickName).HasMaxLength(200).IsRequired(false);
            builder.Property(um => um.DiscordUserName).HasMaxLength(200).IsRequired(true);
            builder.Property(um => um.DiscordMention).HasMaxLength(200).IsRequired(false);
            builder.Property(um => um.DiscordUserId).HasMaxLength(50).IsRequired(true);
            builder.Property(um => um.ServerJoin).IsRequired(false);
            builder.Property(um => um.DiscordJoin).IsRequired(false);

            builder.HasOne(um => um.User)
                .WithMany(u => u.UserMembers)
                .HasForeignKey(um => um.UserId);

            builder.HasOne(um => um.Server)
                .WithMany(s => s.Members)
                .HasForeignKey(um => um.ServerId);

            builder.HasMany(um => um.Snipers)
                .WithOne(s => s.UserMember)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
