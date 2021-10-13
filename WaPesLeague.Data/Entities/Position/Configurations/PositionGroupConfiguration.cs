using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.Position.Configurations
{
    public class PositionGroupConfiguration : IEntityTypeConfiguration<PositionGroup>
    {
        public void Configure(EntityTypeBuilder<PositionGroup> builder)
        {
            builder.Property(pg => pg.Code).HasMaxLength(20).IsRequired(true);
            builder.Property(pg => pg.Name).HasMaxLength(20).IsRequired(false);
            builder.Property(pg => pg.Description).HasMaxLength(200).IsRequired(false);
            builder.Property(pg => pg.Order).IsRequired(true);
            builder.Property(pg => pg.BaseValue).IsRequired(false);
            
            builder.HasIndex(pg => pg.Order).IsUnique(true);

            builder.HasMany(pg => pg.Positions)
                .WithOne(p => p.PositionGroup)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
