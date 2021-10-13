namespace WaPesLeague.Data.Entities.FileImport
{
    public class FileImportRecord
    {
        public int FileImportRecordId { get; set; } 
        public string Record { get; set; }
        public int Row { get; set; }
        public int FileImportId { get; set; }

        public virtual FileImport FileImport { get; set; }
    }
}
