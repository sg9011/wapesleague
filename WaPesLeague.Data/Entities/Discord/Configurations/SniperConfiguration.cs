using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.Discord.Configurations
{
    public class SniperConfiguration : IEntityTypeConfiguration<Sniper>
    {
        public void Configure(EntityTypeBuilder<Sniper> builder)
        {
            builder.Property(ss => ss.DateCreated).IsRequired(true);
            builder.Property(ss => ss.DateEnd).IsRequired(true);

            builder.HasOne(s => s.UserMember)
                .WithMany(um => um.Snipers)
                .HasForeignKey(s => s.UserMemberId);

            builder.HasOne(s => s.InitiatedByServerSniping)
                .WithMany(ss => ss.Snipers)
                .HasForeignKey(s => s.InitiatedByServerSnipingId);

            builder.HasOne(s => s.CatchedOnMixSession)
                .WithMany(ms => ms.Snipers)
                .HasForeignKey(s => s.CatchedOnMixSessionId);
        }
    }
}
