using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaPesLeague.Data.Entities.Association.Enums;

namespace WaPesLeague.Data.Entities.Association.Configurations
{
    public class AssociationConfiguration : IEntityTypeConfiguration<Association>
    {
        public void Configure(EntityTypeBuilder<Association> builder)
        {
            builder.Property(a => a.Name).HasMaxLength(50).IsRequired(true);
            builder.Property(a => a.Code).HasMaxLength(10).IsRequired(true);
            builder.Property(at => at.DefaultTeamType).HasDefaultValue(TeamType.Normal).HasConversion<string>().HasMaxLength(20).IsRequired(true);
            builder.Property(a => a.DateCreated).IsRequired(true);

            builder.HasOne(a => a.AssociationTenant)
                .WithMany(at => at.Associations)
                .HasForeignKey(a => a.AssociationTenantId);

            builder.HasMany(a => a.AssociationLeagueGroups)
                .WithOne(a => a.Association)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(a => a.AssociationTeams)
                .WithOne(at => at.Association)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
