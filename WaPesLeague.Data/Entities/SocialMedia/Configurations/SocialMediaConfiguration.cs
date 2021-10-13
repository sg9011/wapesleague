using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.SocialMedia.Configurations
{
    public class SocialMediaConfiguration : IEntityTypeConfiguration<SocialMedia>
    {
        public void Configure(EntityTypeBuilder<SocialMedia> builder)
        {
            builder.Property(sm => sm.Code).HasMaxLength(50).IsRequired();
            builder.Property(sm => sm.Description).IsRequired(false);

            builder.HasMany(sm => sm.PictureTypes)
                .WithOne(smpt => smpt.SocialMedia)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(sm => sm.UserSocialMedias)
                .WithOne(usm => usm.SocialMedia)
                .OnDelete(DeleteBehavior.Cascade);
            //builder.HasMany(sm => sm.TeamSocialMedias)
            //    .WithOne(tsm => tsm.SocialMedia)
            //    .OnDelete(DeleteBehavior.Cascade);
            //builder.HasMany(sm => sm.MatchSocialMedias)
            //    .WithOne(msm => msm.SocialMedia)
            //    .OnDelete(DeleteBehavior.Cascade);
            //builder.HasMany(sm => sm.FederationSocialMedias)
            //    .WithOne(fsm => fsm.SocialMedia)
            //    .OnDelete(DeleteBehavior.Cascade);
            //builder.HasMany(sm => sm.FederationCompetitionSeasonRoundSocialMedias)
            //    .WithOne(fcsrsm => fcsrsm.SocialMedia)
            //    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
