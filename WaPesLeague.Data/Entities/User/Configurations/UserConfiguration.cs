using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.User.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasIndex(u => u.UserGuid).HasDatabaseName("IX_Contacts_Guid");
            builder.Property(u => u.DiscordName).HasMaxLength(100).IsRequired(false);
            builder.Property(u => u.ExternalId).HasMaxLength(100).IsRequired(false);
            builder.Property(u => u.FirstName).HasMaxLength(50).IsRequired(false);
            builder.Property(u => u.LastName).HasMaxLength(50).IsRequired(false);
            builder.Property(u => u.ExtraInfo).HasMaxLength(2000).IsRequired(false);
            builder.Property(u => u.Email).HasMaxLength(100).IsRequired(false);

            //builder.HasMany(u => u.PictureTypes)
            //    .WithOne(pt => pt.User)
            //    .OnDelete(DeleteBehavior.Cascade);

            //builder.HasMany(u => u.SocialMedias)
            //    .WithOne(sm => sm.User)
            //    .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.UserMetadatas)
                .WithOne(um => um.User)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.PlatformUsers)
                .WithOne(pu => pu.User)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.MixPositionReservations)
                .WithOne(mpr => mpr.User)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.UserMembers)
                .WithOne(um => um.User)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.OwnerOfSessions)
                .WithOne(s => s.RoomOwner)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(u => u.MatchTeamsConfirmedByUser)
                .WithOne(mt => mt.ConfirmedBy)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(u => u.AssociationTenantPlayers)
                .WithOne(atp => atp.User)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.PositionSessionStats)
                .WithOne(mups => mups.User)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
