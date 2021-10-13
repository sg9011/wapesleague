using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.Association.Configurations
{
    public class DivisionRoundConfiguration : IEntityTypeConfiguration<DivisionRound>
    {
        public void Configure(EntityTypeBuilder<DivisionRound> builder)
        {
            builder.Property(dr => dr.Name).HasMaxLength(50).IsRequired(true);
            builder.Property(dr => dr.Order).IsRequired(true);

            builder.HasOne(dr => dr.Division)
                .WithMany(d => d.DivisionRounds)
                .HasForeignKey(dr => dr.DivisionId);

            builder.HasMany(dr => dr.DivisionGroups)
                .WithOne(dg => dg.DivisionRound)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
