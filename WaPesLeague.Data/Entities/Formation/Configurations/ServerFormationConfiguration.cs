using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.Formation.Configurations
{
    class ServerFormationConfiguration : IEntityTypeConfiguration<ServerFormation>
    {
        public void Configure(EntityTypeBuilder<ServerFormation> builder)
        {
            builder.Property(f => f.Name).HasMaxLength(50).IsRequired(true);
            builder.Property(f => f.IsDefault).IsRequired(true);

            builder.HasMany(f => f.Tags)
                .WithOne(ft => ft.Formation);

            builder.HasMany(f => f.Positions)
                .WithOne(p => p.Formation);
        }
    }
}
