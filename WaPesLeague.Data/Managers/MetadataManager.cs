using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Metadata;
using WaPesLeague.Data.Managers.Interfaces;

namespace WaPesLeague.Data.Managers
{
    public class MetadataManager : IMetadataManager
    {
        private readonly WaPesDbContext _context;
        public MetadataManager(WaPesDbContext context)
        {
            _context = context;
        }

        public async Task<Metadata> AddAsync(Metadata metadata)
        {
            await _context.Metadatas.AddAsync(metadata);
            await _context.SaveChangesAsync();

            return metadata;
        }

        public async Task<List<Metadata>> GetAllAsync()
        {
            return await _context.Metadatas
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
