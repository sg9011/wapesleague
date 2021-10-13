using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.Platform.Configurations
{
    public class PlatformConfiguration : IEntityTypeConfiguration<Platform>
    {
        public void Configure(EntityTypeBuilder<Platform> builder)
        {
            builder.Property(p => p.Name).HasMaxLength(50).IsRequired();
            builder.Property(p => p.Description).IsRequired(false);

            builder.HasMany(p => p.PlatformUsers)
                .WithOne(pu => pu.Platform)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
