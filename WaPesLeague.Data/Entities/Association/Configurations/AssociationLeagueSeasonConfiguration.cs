using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.Association.Configurations
{
    public class AssociationLeagueSeasonConfiguration : IEntityTypeConfiguration<AssociationLeagueSeason>
    {
        public void Configure(EntityTypeBuilder<AssociationLeagueSeason> builder)
        {
            builder.Property(als => als.Name).HasMaxLength(50).IsRequired(true);
            builder.Property(als => als.Edition).HasMaxLength(20).IsRequired(true);
            builder.Property(als => als.Order).IsRequired(true);
            builder.Property(als => als.Start).IsRequired(true);
            builder.Property(als => als.End).IsRequired(false);

            builder.HasOne(als => als.AssociationLeagueGroup)
                .WithMany(alg => alg.AssociationLeagueSeasons)
                .HasForeignKey(als => als.AssociationLeagueGroupId);

            builder.HasMany(als => als.Divisions)
                .WithOne(d => d.AssociationLeagueSeason)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
