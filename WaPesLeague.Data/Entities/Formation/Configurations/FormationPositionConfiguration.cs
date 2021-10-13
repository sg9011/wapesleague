using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.Formation.Configurations
{
    public class FormationPositionConfiguration : IEntityTypeConfiguration<FormationPosition>
    {
        public void Configure(EntityTypeBuilder<FormationPosition> builder)
        {
             builder.HasOne(fp => fp.Position)
                .WithMany(p => p.FormationPositions)
                .HasForeignKey(fp => fp.PositionId);

            builder.HasOne(fp => fp.Formation)
                .WithMany(f=> f.FormationPositions)
                .HasForeignKey(fp => fp.FormationId);
        }
    }
}
