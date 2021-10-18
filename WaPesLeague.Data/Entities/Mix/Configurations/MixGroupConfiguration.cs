using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.Mix.Configurations
{
    public class MixGroupConfiguration : IEntityTypeConfiguration<MixGroup>
    {
        public void Configure(EntityTypeBuilder<MixGroup> builder)
        {
            builder.Property(mc => mc.ExtraInfo).HasMaxLength(500);
            builder.Property(mc => mc.Recurring).IsRequired(true);
            builder.Property(mc => mc.CreateExtraMixChannels).IsRequired(true);
            builder.Property(mc => mc.Start).HasPrecision(18, 4).IsRequired(true);
            builder.Property(mc => mc.HoursToOpenRegistrationBeforeStart).HasPrecision(18, 4).IsRequired(true);
            builder.Property(mc => mc.MaxSessionDurationInHours).HasPrecision(18, 4).IsRequired(true);
            builder.Property(mc => mc.IsActive).IsRequired(true);

            builder.HasMany(mx => mx.MixChannels)
                .WithOne(mc => mc.MixGroup)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(mx => mx.MixGroupRoleOpenings)
                .WithOne(mc => mc.MixGroup)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(mx => mx.Server)
                .WithMany(s => s.MixGroups)
                .HasForeignKey(mx => mx.ServerId);
        }
    }
}
