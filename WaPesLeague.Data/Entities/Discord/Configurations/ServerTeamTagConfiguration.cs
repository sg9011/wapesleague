using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.Discord.Configurations
{
    public class ServerTeamTagConfiguration : IEntityTypeConfiguration<ServerTeamTag>
    {
        public void Configure(EntityTypeBuilder<ServerTeamTag> builder)
        {
            builder.Property(stt => stt.Tag).HasMaxLength(50).IsRequired(true);

            builder.HasOne(stt => stt.Team)
                .WithMany(st => st.Tags)
                .HasForeignKey(stt => stt.ServerTeamId);
        }
    }
}
