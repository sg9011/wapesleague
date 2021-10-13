using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.Association.Configurations
{
    public class AssociationTeamConfiguration : IEntityTypeConfiguration<AssociationTeam>
    {
        public void Configure(EntityTypeBuilder<AssociationTeam> builder)
        {
            builder.Property(at => at.Guid).IsRequired(true);
            builder.Property(at => at.Name).HasMaxLength(50).IsRequired(false);
            builder.Property(at => at.Description).IsRequired(false);
            builder.Property(at => at.TeamType).HasConversion<string>().HasMaxLength(20).IsRequired(true);
            builder.Property(at => at.DateCreated).IsRequired(true);

            builder.HasOne(at => at.Association)
                .WithMany(a => a.AssociationTeams)
                .HasForeignKey(at => at.AssociationId);

            builder.HasMany(at => at.AssociationTeamPlayers)
                .WithOne(atp => atp.AssociationTeam)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(at => at.MatchTeams)
                .WithOne(mt => mt.Team)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
