using System.Collections.Generic;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.FileImport;
using WaPesLeague.Data.Managers.FileImport.Intefaces;

namespace WaPesLeague.Data.Managers.FileImport
{
    public class FileImportRecordManager : IFileImportRecordManager
    {
        private readonly WaPesDbContext _context;
        public FileImportRecordManager(WaPesDbContext context)
        {
            _context = context;
        }

        public async Task<List<FileImportRecord>> AddMultipleAsync(List<FileImportRecord> fileImportRecords)
        {
            await _context.FileImportRecords.AddRangeAsync(fileImportRecords);
            await _context.SaveChangesAsync();

            return fileImportRecords;
        }
    }
}
