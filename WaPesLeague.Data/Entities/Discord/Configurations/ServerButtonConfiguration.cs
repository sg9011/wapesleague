using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.Discord.Configurations
{
    public class ServerButtonConfiguration : IEntityTypeConfiguration<ServerButton>
    {
        public void Configure(EntityTypeBuilder<ServerButton> builder)
        {
            builder.Property(b => b.Message).HasMaxLength(100).IsRequired(true);
            builder.Property(b => b.URL).HasMaxLength(400).IsRequired(true);
            builder.Property(b => b.ShowFrom).IsRequired(false);
            builder.Property(b => b.ShowUntil).IsRequired(false);

            builder.HasOne(b => b.ButtonGroup)
                .WithMany(bg => bg.Buttons)
                .HasForeignKey(b => b.ServerButtonGroupId);
        }
    }
}
