using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaPesLeague.Data.Managers.FileImport.Intefaces;
using FI = WaPesLeague.Data.Entities.FileImport;

namespace WaPesLeague.Data.Managers.FileImport
{
    public class FileImportManager : IFileImportManager
    {
        private readonly WaPesDbContext _context;
        public FileImportManager(WaPesDbContext context)
        {
            _context = context;
        }

        public async Task<FI.FileImport> GetByIdAsync(int fileImportId)
        {
            return await _context.FileImports
                .Include(fi => fi.FileImportRecords)
                .AsNoTracking()
                .FirstOrDefaultAsync(fi => fi.FileImportId == fileImportId);
        }

        public async Task<IReadOnlyCollection<FI.FileImport>> GetAllSuccessfulFileImports()
        {
            return await _context.FileImports
                .Where(fi => fi.FileStatus == FI.Enums.FileStatus.Success)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<FI.FileImport> AddAsync(FI.FileImport fileImport)
        {
            await _context.FileImports.AddAsync(fileImport);
            await _context.SaveChangesAsync();

            return fileImport;
        }

        public async Task<FI.FileImport> UpdateAsync(FI.FileImport fileImport)
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
