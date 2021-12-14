using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.User.Configurations
{
    public class UserPictureTypeConfiguration : IEntityTypeConfiguration<UserPictureType>
    {
        public void Configure(EntityTypeBuilder<UserPictureType> builder)
        {
            builder.Property(upt => upt.Link).HasMaxLength(400).IsRequired(true);
            builder.Property(upt => upt.Description).HasMaxLength(400).IsRequired(false);
            //builder.HasOne(upt => upt.User)
            //    .WithMany(u => u.PictureTypes)
            //    .HasForeignKey(upt => upt.UserPictureTypeId);
            builder.HasOne(upt => upt.PictureType)
                .WithMany(pt => pt.UserPictureTypes)
                .HasForeignKey(upt => upt.PictureTypeId);
        }
    }
}
