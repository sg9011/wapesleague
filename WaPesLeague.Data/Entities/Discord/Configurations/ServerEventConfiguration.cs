using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.Discord.Configurations
{
    public class ServerEventConfiguration : IEntityTypeConfiguration<ServerEvent>
    {
        public void Configure(EntityTypeBuilder<ServerEvent> builder)
        {
            builder.Property(se => se.EventType).HasMaxLength(100).HasConversion<string>();
            builder.Property(se => se.ActionType).HasMaxLength(100).HasConversion<string>();
            builder.Property(se => se.ActionEntity).HasMaxLength(100).HasConversion<string>();

            builder.HasOne(se => se.Server)
                .WithMany(s => s.ServerEvents)
                .HasForeignKey(se => se.ServerId);
        }
    }
}
