using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.Match.Configurations
{
    public class MatchConfiguration : IEntityTypeConfiguration<Match>
    {
        public void Configure(EntityTypeBuilder<Match> builder)
        {
            builder.Property(m => m.Guid).IsRequired(true);
            builder.Property(m => m.Order).IsRequired(true);
            builder.Property(m => m.DateCreated).IsRequired(true);
            builder.Property(m => m.DatePlanned).IsRequired(false);
            builder.Property(m => m.DatePlayed).IsRequired(false);
            builder.Property(m => m.MatchStatus).HasConversion<string>().HasMaxLength(20).IsRequired(true);

            builder.HasOne(m => m.DivisionGroupRound)
                .WithMany(dgr => dgr.Matches)
                .HasForeignKey(m => m.DivisionGroupRoundId);

            builder.HasMany(m => m.MatchTeams)
                .WithOne(mt => mt.Match)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
