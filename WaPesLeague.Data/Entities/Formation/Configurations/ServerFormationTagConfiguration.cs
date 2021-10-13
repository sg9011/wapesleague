using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace WaPesLeague.Data.Entities.Formation.Configurations
{
    class ServerFormationTagConfiguration : IEntityTypeConfiguration<ServerFormationTag>
    {
        public void Configure(EntityTypeBuilder<ServerFormationTag> builder)
        {
            builder.Property(ft => ft.Tag).HasMaxLength(20).IsRequired(true);

            builder.HasOne(ft => ft.Formation)
                .WithMany(f => f.Tags)
                .HasForeignKey(ft => ft.ServerFormationId);
        }
    }
}
