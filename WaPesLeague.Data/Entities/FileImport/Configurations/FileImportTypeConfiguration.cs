using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.FileImport.Configurations
{
    public class FileImportTypeConfiguration : IEntityTypeConfiguration<GoogleSheetImportType>
    {
        public void Configure(EntityTypeBuilder<GoogleSheetImportType> builder)
        {
            builder.Property(a => a.Code).HasMaxLength(50).IsRequired(true);
            builder.Property(a => a.StartRow).IsRequired(true).HasDefaultValue(0);
            builder.Property(a => a.TabName).IsRequired(true).HasMaxLength(50);

            builder.HasMany(a => a.GoogleSheetImports)
                .WithOne(at => at.FileImportType)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
