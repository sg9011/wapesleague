using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.Formation.Configurations
{
    public class FormationTagConfiguration : IEntityTypeConfiguration<FormationTag>
    {
        public void Configure(EntityTypeBuilder<FormationTag> builder)
        {
            builder.Property(ft => ft.Tag).HasMaxLength(20).IsRequired(true); //This is actually a fake to be fair
            builder.HasIndex(ft => new { ft.Tag }).IsUnique(true);

            builder.HasOne(ft => ft.Formation)
                .WithMany(f => f.Tags)
                .HasForeignKey(ft => ft.FormationId);
        }
    }
}
