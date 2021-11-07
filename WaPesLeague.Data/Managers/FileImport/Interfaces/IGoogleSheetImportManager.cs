using System.Collections.Generic;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.FileImport;

namespace WaPesLeague.Data.Managers.FileImport.Intefaces
{
    public interface IGoogleSheetImportManager
    {
        public Task<IReadOnlyCollection<GoogleSheetImportType>> GetAll();
    }
}
