using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.PictureType.Configurations
{
    public class PictureTypeConfiguration : IEntityTypeConfiguration<PictureType>
    {
        public void Configure(EntityTypeBuilder<PictureType> builder)
        {
            builder.Property(pt => pt.Code).HasMaxLength(50).IsRequired();
            builder.Property(pt => pt.Description).IsRequired(false);

            builder.HasMany(pt => pt.UserPictureTypes)
                .WithOne(upt => upt.PictureType)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(pt => pt.SocialMediaPictureTypes)
                .WithOne(smpt => smpt.PictureType)
                .OnDelete(DeleteBehavior.Cascade);
            //builder.HasMany(pt => pt.MatchPictureTypes)
            //    .WithOne(mpt => mpt.PictureType)
            //    .OnDelete(DeleteBehavior.Cascade);
            //builder.HasMany(pt => pt.TeamPictureTypes)
            //    .WithOne(tpt => tpt.PictureType)
            //    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
