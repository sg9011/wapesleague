using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.Position.Configurations
{
    public class PositionTagConfiguration : IEntityTypeConfiguration<PositionTag>
    {
        public void Configure(EntityTypeBuilder<PositionTag> builder)
        {
            builder.Property(pt => pt.Tag).HasMaxLength(10).IsRequired(true);
            builder.Property(pt => pt.ServerId).IsRequired(true);

            builder.HasIndex(pt => new { pt.Tag, pt.ServerId }).IsUnique(true);
            builder.HasIndex(pt => new { pt.PositionId, pt.ServerId });
            
            builder.HasOne(pt => pt.Position)
                .WithMany(p => p.Tags)
                .HasForeignKey(pt => pt.PositionId);

            builder.HasOne(pt => pt.Server)
                .WithMany(s => s.PositionTags)
                .HasForeignKey(pt => pt.ServerId);
        }
    }
}
