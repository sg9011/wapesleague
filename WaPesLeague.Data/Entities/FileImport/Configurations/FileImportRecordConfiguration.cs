using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaPesLeague.Data.Entities.FileImport.Configurations
{
    public class FileImportRecordConfiguration : IEntityTypeConfiguration<FileImportRecord>
    {
        public void Configure(EntityTypeBuilder<FileImportRecord> builder)
        {
            builder.Property(fir => fir.Record).IsRequired(true);
            builder.Property(fir => fir.Row).IsRequired(true);

            builder.HasOne(fir => fir.FileImport)
                .WithMany(fi => fi.FileImportRecords)
                .HasForeignKey(als => als.FileImportId);

        }
    }
}
