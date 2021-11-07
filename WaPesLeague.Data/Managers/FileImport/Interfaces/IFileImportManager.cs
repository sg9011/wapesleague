using System.Threading.Tasks;

namespace WaPesLeague.Data.Managers.FileImport.Intefaces
{
    public interface IFileImportManager
    {
        public Task<Entities.FileImport.FileImport> GetByIdAsync(int fileImportId);
        public Task<Entities.FileImport.FileImport> AddAsync(Entities.FileImport.FileImport fileImport);
        public Task<Entities.FileImport.FileImport> UpdateAsync(Entities.FileImport.FileImport fileImport);

    }
}
