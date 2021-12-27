using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.FileImport;
using WaPesLeague.Data.Managers.FileImport.Intefaces;

namespace WaPesLeague.Data.Managers.FileImport
{
    public class GoogleSheetImportManager : IGoogleSheetImportManager
    {
        private readonly WaPesDbContext _context;

        public GoogleSheetImportManager(WaPesDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyCollection<GoogleSheetImportType>> GetAll()
        {
            return await _context.GoogleSheetImportTypes
                .Include(gs => gs.GoogleSheetImports)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
