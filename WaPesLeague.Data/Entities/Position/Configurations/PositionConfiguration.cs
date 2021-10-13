using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.Position.Configurations
{
    public class PositionConfiguration : IEntityTypeConfiguration<Position>
    {
        public void Configure(EntityTypeBuilder<Position> builder)
        {
            builder.Property(p => p.Order).IsRequired(true);
            builder.Property(p => p.Code).HasMaxLength(5).IsRequired(true);
            //builder.Property(p => p.Category).HasMaxLength(5).IsRequired(false);
            builder.Property(p => p.Description).HasMaxLength(200).IsRequired(false);
            builder.Property(p => p.IsRequiredForMix).IsRequired(true);
            builder.Property(p => p.ParentPositionId).IsRequired(false);
            builder.HasIndex(p => new { p.PositionGroupId, p.Order}).IsUnique(true);

            builder.HasOne(p => p.PositionGroup)
                .WithMany(pg => pg.Positions)
                .HasForeignKey(p => p.PositionGroupId);

            builder.HasMany(p => p.MixChannelPositions)
                .WithOne(mcp => mcp.Position)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.MixPositions)
                .WithOne(mp => mp.Position);

            builder.HasMany(p => p.Tags)
                .WithOne(pt => pt.Position);

            builder.HasMany(p => p.FormationPositions)
                .WithOne(fp => fp.Position);
            builder.HasMany(p => p.ServerFormationPositions)
                .WithOne(fp => fp.Position);

            builder.HasOne(p => p.ParentPosition)
                .WithMany(pp => pp.ChildPositions)
                .HasForeignKey(p => p.ParentPositionId);

            builder.HasMany(p => p.UserSessionStats)
                .WithOne(mups => mups.Position)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
