using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaPesLeague.Data.Entities.Metadata.Enums;

namespace WaPesLeague.Data.Entities.Metadata.Configurations
{
    public class MetadataConfiguration : IEntityTypeConfiguration<Metadata>
    {
        public void Configure(EntityTypeBuilder<Metadata> builder)
        {
            builder.Property(m => m.Code).HasMaxLength(50).IsRequired();
            builder.Property(m => m.Description).HasMaxLength(400).IsRequired();
            builder.Property(m => m.PropertyType).HasConversion<string>().HasDefaultValue(PropertyType.String).HasMaxLength(30).IsRequired(true);

            builder.HasMany(m => m.UserMetadatas)
                .WithOne(um => um.Metadata)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
