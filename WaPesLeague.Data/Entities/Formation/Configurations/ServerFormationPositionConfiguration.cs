using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.Formation.Configurations
{
    public class ServerFormationPositionConfiguration : IEntityTypeConfiguration<ServerFormationPosition>
    {
        public void Configure(EntityTypeBuilder<ServerFormationPosition> builder)
        {
            builder.HasOne(fp => fp.Position)
               .WithMany(p => p.ServerFormationPositions)
               .HasForeignKey(fp => fp.PositionId);

            builder.HasOne(fp => fp.Formation)
                .WithMany(f => f.Positions)
                .HasForeignKey(fp => fp.ServerFormationId);
        }
    }
}
