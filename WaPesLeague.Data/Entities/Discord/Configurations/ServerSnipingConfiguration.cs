using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.Discord.Configurations
{
    public class ServerSnipingConfiguration : IEntityTypeConfiguration<ServerSniping>
    {
        public void Configure(EntityTypeBuilder<ServerSniping> builder)
        {
            builder.Property(ss => ss.IntervalAfterRegistrationOpeningInMinutes).IsRequired(true);
            builder.Property(ss => ss.SignUpDelayInMinutes).IsRequired(true);
            builder.Property(ss => ss.SignUpDelayDurationInHours).IsRequired(true);

            builder.HasOne(ss => ss.Server)
                .WithMany(s => s.ServerSnipings)
                .HasForeignKey(ss => ss.ServerId);

            builder.HasMany(ss => ss.Snipers)
                .WithOne(s => s.InitiatedByServerSniping)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
