using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.FileImport.Configurations
{
    public class FileImportConfiguration : IEntityTypeConfiguration<FileImport>
    {
        public void Configure(EntityTypeBuilder<FileImport> builder)
        {
            builder.Property(fi => fi.FileStatus).HasConversion<string>().HasMaxLength(20).IsRequired(true);
            builder.Property(fi => fi.DateCreated).IsRequired(true);
            builder.Property(fi => fi.ErrorMessage).IsRequired(true);

            builder.HasOne(fi => fi.FileImportType)
                .WithMany(fit => fit.GoogleSheetImports)
                .HasForeignKey(fi => fi.FileImportTypeId);

            builder.HasMany(fi => fi.FileImportRecords)
                .WithOne(fir => fir.FileImport)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
