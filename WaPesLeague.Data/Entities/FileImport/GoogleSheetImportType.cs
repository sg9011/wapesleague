using System.Collections.Generic;
using WaPesLeague.Data.Entities.Association;
using WaPesLeague.Data.Entities.FileImport.Enums;

namespace WaPesLeague.Data.Entities.FileImport
{
    public class GoogleSheetImportType
    {
        public int GoogleSheetImportTypeId { get; set; }
        public string Code { get; set; }
        public RecordType RecordType { get; set; }
        public bool HasTitleRow { get; set; }
        public string Range { get; set; }
        public string GoogleSheetName { get; set; }
        public string TabName { get; set; }
        public string GoogleSheetId { get; set; }

        public virtual List<FileImport> GoogleSheetImports { get; set; }
        public virtual DivisionGroup DivisionGroup { get; set; }
    }
}
