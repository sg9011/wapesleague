using System.Collections.Generic;
using System.Threading.Tasks;

namespace WaPesLeague.Data.Managers.FileImport.Intefaces
{
    public interface IFileImportRecordManager
    {
        public Task<List<Entities.FileImport.FileImportRecord>> AddMultipleAsync(List<Entities.FileImport.FileImportRecord> fileImportRecords);
    }
}
