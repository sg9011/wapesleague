using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.Mix.Configurations
{
    public class MixPositionReservationConfiguration : IEntityTypeConfiguration<MixPositionReservation>
    {
        public void Configure(EntityTypeBuilder<MixPositionReservation> builder)
        {
            builder.Property(mpr => mpr.DateStart).IsRequired(true);
            builder.Property(mpr => mpr.DateEnd).IsRequired(false);
            builder.Property(mpr => mpr.ExtraInfo).HasMaxLength(50).IsRequired(false);
            builder.Property(mpr => mpr.IsCaptain).IsRequired(true);

            builder.HasOne(mpr => mpr.User)
                .WithMany(u => u.MixPositionReservations)
                .HasForeignKey(mpr => mpr.UserId);

            builder.HasOne(mpr => mpr.MixPosition)
                .WithMany(mp => mp.Reservations)
                .HasForeignKey(r => r.MixPositionId);
        }
    }
}
