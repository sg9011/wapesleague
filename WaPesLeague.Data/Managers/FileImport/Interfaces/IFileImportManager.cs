using System.Collections.Generic;
using System.Threading.Tasks;
using FI = WaPesLeague.Data.Entities.FileImport;

namespace WaPesLeague.Data.Managers.FileImport.Intefaces
{
    public interface IFileImportManager
    {
        public Task<FI.FileImport> GetByIdAsync(int fileImportId);
        public Task<IReadOnlyCollection<FI.FileImport>> GetAllSuccessfulFileImports();
        public Task<Entities.FileImport.FileImport> AddAsync(FI.FileImport fileImport);
        public Task<Entities.FileImport.FileImport> UpdateAsync(FI.FileImport fileImport);

    }
}
