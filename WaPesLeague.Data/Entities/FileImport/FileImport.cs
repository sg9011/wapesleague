using System;
using System.Collections.Generic;
using WaPesLeague.Data.Entities.FileImport.Enums;

namespace WaPesLeague.Data.Entities.FileImport
{
    public class FileImport
    {
        public int FileImportId { get; set; }
        public int FileImportTypeId { get; set; }
        public FileStatus FileStatus { get; set; }
        public RecordType RecordType { get; set; }
        public DateTime DateCreated { get; set; }
        public string ErrorMessage { get; set; }

        public virtual GoogleSheetImportType FileImportType { get; set; }
        public virtual List<FileImportRecord> FileImportRecords { get; set; }
    }
}
