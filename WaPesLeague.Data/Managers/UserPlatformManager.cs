using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.User;
using WaPesLeague.Data.Managers.Interfaces;

namespace WaPesLeague.Data.Managers
{
    public class UserPlatformManager : IUserPlatformManager
    {
        private readonly WaPesDbContext _context;
        public UserPlatformManager(WaPesDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyCollection<UserPlatform>> GetUserPlatformsByUserId(int userId)
        {
            return await _context.UserPlatforms
                .Include(up => up.Platform)
                .Where(up => up.UserId == userId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<UserPlatform> GetUserPlatformByUserId(int userId, int platformId)
        {
            return await _context.UserPlatforms
                .AsNoTracking()
                .FirstOrDefaultAsync(up => up.UserId == userId && up.PlatformId == platformId);
        }

        public async Task<UserPlatform> AddAsync(UserPlatform userPlatform)
        {
            await _context.UserPlatforms.AddAsync(userPlatform);
            await _context.SaveChangesAsync();

            return userPlatform;
        }

        public async Task<UserPlatform> UpdateAsync(UserPlatform userPlatform)
        {
            var currentUserPlatform = await _context.UserPlatforms.FindAsync(userPlatform.UserPlatformId);
            if (currentUserPlatform != null)
            {
                _context.Entry(currentUserPlatform).CurrentValues.SetValues(userPlatform);
                await _context.SaveChangesAsync();
            }
            return currentUserPlatform;
        }
    }
}
