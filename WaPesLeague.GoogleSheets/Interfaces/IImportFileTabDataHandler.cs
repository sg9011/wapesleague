using System.Collections.Generic;
using System.Threading.Tasks;

namespace WaPesLeague.GoogleSheets.Interfaces
{
    public interface IImportFileTabDataHandler
    {
        public Task<List<ImportFileTabRecord>> HandleAsync(string fileName, string tab, string fileId);
    }
}
