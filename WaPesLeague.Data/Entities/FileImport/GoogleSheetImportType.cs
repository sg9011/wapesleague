using System.Collections.Generic;

namespace WaPesLeague.Data.Entities.FileImport
{
    public class GoogleSheetImportType
    {
        public int GoogleSheetImportTypeId { get; set; }
        public string Code { get; set; }
        public int StartRow { get; set; }
        public string GoogleSheetName { get; set; }
        public string TabName { get; set; }
        public string GoogleSheetId { get; set; }

        public virtual List<FileImport> GoogleSheetImports { get; set; }
    }
}
