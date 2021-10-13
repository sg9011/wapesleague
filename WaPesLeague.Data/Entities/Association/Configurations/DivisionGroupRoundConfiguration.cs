using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
namespace WaPesLeague.Data.Entities.Association.Configurations
{
    public class DivisionGroupRoundConfiguration : IEntityTypeConfiguration<DivisionGroupRound>
    {
        public void Configure(EntityTypeBuilder<DivisionGroupRound> builder)
        {
            builder.Property(dgr => dgr.Name).HasMaxLength(50).IsRequired(true);
            builder.Property(dgr => dgr.Order).IsRequired(true);

            builder.Property(dgr => dgr.Start).IsRequired(true);
            builder.Property(dgr => dgr.End).IsRequired(false);
            builder.Property(dgr => dgr.DateCreated).IsRequired(true);

            builder.HasOne(dgr => dgr.DivisionGroup)
                .WithMany(dg => dg.DivisionGroupRounds)
                .HasForeignKey(dgr => dgr.DivisionGroupId);

            builder.HasMany(dgr => dgr.Matches)
                .WithOne(m => m.DivisionGroupRound)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
