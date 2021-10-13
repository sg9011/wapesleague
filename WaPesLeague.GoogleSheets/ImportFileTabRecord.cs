using System.Collections.Generic;

namespace WaPesLeague.GoogleSheets
{
    public class ImportFileTabRecord
    {
        public int Row { get; set; }
        public Dictionary<string, object> Data { get; set; }
    }
}
