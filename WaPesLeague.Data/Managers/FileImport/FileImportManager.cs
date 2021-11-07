using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WaPesLeague.Data.Managers.FileImport.Intefaces;

namespace WaPesLeague.Data.Managers.FileImport
{
    public class FileImportManager : IFileImportManager
    {
        private readonly WaPesDbContext _context;
        public FileImportManager(WaPesDbContext context)
        {
            _context = context;
        }

        public async Task<Entities.FileImport.FileImport> GetByIdAsync(int fileImportId)
        {
            return await _context.FileImports
                .Include(fi => fi.FileImportRecords)
                .AsNoTracking()
                .FirstOrDefaultAsync(fi => fi.FileImportId == fileImportId);
        }

        public async Task<Entities.FileImport.FileImport> AddAsync(Entities.FileImport.FileImport fileImport)
        {
            await _context.FileImports.AddAsync(fileImport);
            await _context.SaveChangesAsync();

            return fileImport;
        }

        public async Task<Entities.FileImport.FileImport> UpdateAsync(Entities.FileImport.FileImport fileImport)
        {
            var currentFileImport = await _context.FileImports.FindAsync(fileImport.FileImportId);
            if (currentFileImport != null)
            {
                _context.Entry(currentFileImport).CurrentValues.SetValues(fileImport);
                await _context.SaveChangesAsync();
            }
            return currentFileImport;
        }
    }
}
