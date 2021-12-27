using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.User.Configurations
{
    public class UserMetadataConfiguration : IEntityTypeConfiguration<UserMetadata>
    {
        public void Configure(EntityTypeBuilder<UserMetadata> builder)
        {
            builder.Property(um => um.Value).HasMaxLength(200).IsRequired(true);

            builder.HasOne(um => um.Metadata)
                .WithMany(m => m.UserMetadatas)
                .HasForeignKey(um => um.MetadataId);
        }
    }
}
