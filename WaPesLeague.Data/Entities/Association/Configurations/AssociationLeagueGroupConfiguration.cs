using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.Association.Configurations
{
    public class AssociationLeagueGroupConfiguration : IEntityTypeConfiguration<AssociationLeagueGroup>
    {
        public void Configure(EntityTypeBuilder<AssociationLeagueGroup> builder)
        {
            builder.Property(alg => alg.Name).HasMaxLength(50).IsRequired(true);
            builder.Property(alg => alg.Code).HasMaxLength(10).IsRequired(true);
            builder.Property(alg => alg.DateCreated).IsRequired(true);

            builder.HasOne(alg => alg.Association)
                .WithMany(a => a.AssociationLeagueGroups)
                .HasForeignKey(alg => alg.AssociationId);

            builder.HasMany(alg => alg.AssociationLeagueSeasons)
                .WithOne(als => als.AssociationLeagueGroup)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
