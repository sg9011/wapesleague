using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.User.Configurations
{
    public class UserPlatformConfiguration : IEntityTypeConfiguration<UserPlatform>
    {
        public void Configure(EntityTypeBuilder<UserPlatform> builder)
        {
            builder.Property(up => up.UserName).HasMaxLength(100).IsRequired(false);

            builder.HasOne(up => up.Platform)
                .WithMany(p => p.PlatformUsers)
                .HasForeignKey(up => up.PlatformId);

            builder.HasOne(up => up.User)
                .WithMany(p => p.PlatformUsers)
                .HasForeignKey(up => up.UserId);
        }
    }
}
