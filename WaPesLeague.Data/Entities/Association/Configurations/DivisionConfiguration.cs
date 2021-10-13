using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.Association.Configurations
{
    public class DivisionConfiguration : IEntityTypeConfiguration<Division>
    {
        public void Configure(EntityTypeBuilder<Division> builder)
        {
            builder.Property(d => d.Order).IsRequired(true);

            builder.HasOne(d => d.AssociationLeagueSeason)
                .WithMany(als => als.Divisions)
                .HasForeignKey(d => d.AssociationLeagueSeasonId);

            builder.HasMany(d => d.DivisionRounds)
                .WithOne(dr => dr.Division)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
