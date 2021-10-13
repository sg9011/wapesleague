using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.User.Configurations
{
    public class UserSocialMediaConfiguration: IEntityTypeConfiguration<UserSocialMedia>
    {
        public void Configure(EntityTypeBuilder<UserSocialMedia> builder)
        {
            builder.Property(usm => usm.Link).HasMaxLength(400).IsRequired(true);
            builder.Property(usm => usm.Description).HasMaxLength(400).IsRequired(false);
            //builder.HasOne(usm => usm.User)
            //    .WithMany(u => u.SocialMedias)
            //    .HasForeignKey(usm => usm.UserSocialMediaId);
            builder.HasOne(usm => usm.SocialMedia)
                .WithMany(sm => sm.UserSocialMedias)
                .HasForeignKey(usm => usm.SocialMediaId);
        }
    }
}
