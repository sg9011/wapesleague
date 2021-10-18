using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.Mix.Configurations
{
    public class MixGroupRoleOpeningConfiguration : IEntityTypeConfiguration<MixGroupRoleOpening>
    {
        public void Configure(EntityTypeBuilder<MixGroupRoleOpening> builder)
        {
            builder.Property(mc => mc.Minutes).IsRequired(true);
            builder.Property(mc => mc.DateCreated).IsRequired(true);
            builder.Property(mc => mc.DateEnd).IsRequired(false);
            builder.Property(mc => mc.IsActive).IsRequired(true);

            builder.HasOne(mx => mx.ServerRole)
                .WithMany(s => s.MixGroupRoleOpenings)
                .HasForeignKey(mgro => mgro.ServerRoleId);

            builder.HasOne(mx => mx.MixGroup)
                .WithMany(s => s.MixGroupRoleOpenings)
                .HasForeignKey(mx => mx.MixGroupId);
        }
    }
}
