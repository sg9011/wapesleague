using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Platform;
using WaPesLeague.Data.Managers.Interfaces;

namespace WaPesLeague.Data.Managers
{
    public class PlatformManager : IPlatformManager
    {
        private readonly WaPesDbContext _context;
        public PlatformManager(WaPesDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyCollection<Platform>> GetPlatformsAsync()
        {
            return await ReadOnlyPlatforms().ToListAsync();
        }
        public async Task<Platform> GetPlatformWithPlatformUserIdsByNameAsync(string name, string discordServerId)
        {
            return await _context.Platforms
                .Include(p => p.PlatformUsers.Where(pu => pu.User.UserMembers.Any(um => um.Server.DiscordServerId == discordServerId && um.Server.IsActive == true)))
                .AsNoTracking()
                .SingleOrDefaultAsync(p => p.Name == name);
        }

        private IQueryable<Platform> ReadOnlyPlatforms()
        {
            return _context.Platforms.AsNoTracking();
        }
    }
}
