using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.Formation.Configurations
{
    public class FormationConfiguration : IEntityTypeConfiguration<Formation>
    {
        public void Configure(EntityTypeBuilder<Formation> builder)
        {
            builder.Property(f => f.Name).HasMaxLength(50).IsRequired(true);
            builder.Property(f => f.IsDefault).IsRequired(true);

            builder.HasMany(f => f.Tags)
                .WithOne(ft => ft.Formation);

            builder.HasMany(f => f.FormationPositions)
                .WithOne(p => p.Formation);
        }
    }
}
