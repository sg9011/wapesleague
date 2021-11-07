using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.FileImport.Configurations
{
    public class GoogleSheetTypeConfiguration : IEntityTypeConfiguration<GoogleSheetImportType>
    {
        public void Configure(EntityTypeBuilder<GoogleSheetImportType> builder)
        {
            builder.Property(gs => gs.Code).HasMaxLength(50).IsRequired(true);
            builder.Property(gs => gs.Range).IsRequired(true);
            builder.Property(gs => gs.RecordType).HasMaxLength(100).HasConversion<string>();
            builder.Property(gs => gs.TabName).IsRequired(true).HasMaxLength(50);
            builder.Property(gs => gs.GoogleSheetId).IsRequired(true).HasMaxLength(50);

            builder.HasMany(gs => gs.GoogleSheetImports)
                .WithOne(at => at.FileImportType)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
