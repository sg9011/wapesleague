using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.Association.Configurations
{
    public class DivisionGroupConfiguration : IEntityTypeConfiguration<DivisionGroup>
    {
        public void Configure(EntityTypeBuilder<DivisionGroup> builder)
        {
            builder.Property(dg => dg.Name).HasMaxLength(50).IsRequired(true);
            builder.Property(dg => dg.Order).IsRequired(true);

            builder.HasOne(dg => dg.DivisionRound)
                .WithMany(dr => dr.DivisionGroups)
                .HasForeignKey(dg => dg.DivisionRoundId);

            builder.HasMany(dg => dg.DivisionGroupRounds)
                .WithOne(dgr => dgr.DivisionGroup)
                .OnDelete(DeleteBehavior.Cascade);
                
        }
    }
}
