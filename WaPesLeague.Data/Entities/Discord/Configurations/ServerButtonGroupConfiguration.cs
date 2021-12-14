using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaPesLeague.Data.Entities.Discord.Enums;

namespace WaPesLeague.Data.Entities.Discord.Configurations
{
    public class ServerButtonGroupConfiguration : IEntityTypeConfiguration<ServerButtonGroup>
    {
        public void Configure(EntityTypeBuilder<ServerButtonGroup> builder)
        {
            builder.Property(bg => bg.UseRate).IsRequired(true).HasPrecision(18, 8);
            builder.Property(gs => gs.ButtonGroupType).HasMaxLength(100).HasConversion<string>().HasDefaultValue(ButtonGroupType.ShowAllAtTheSameTime);

            builder.HasMany(bg => bg.Buttons)
                .WithOne(b => b.ButtonGroup)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(bg => bg.Server)
                .WithMany(s => s.ButtonGroups)
                .HasForeignKey(bg => bg.ServerId);
        }
    }
}
