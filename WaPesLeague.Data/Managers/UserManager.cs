using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.User;
using WaPesLeague.Data.Managers.Interfaces;

namespace WaPesLeague.Data.Managers
{
    public class UserManager : IUserManager
    {
        private readonly WaPesDbContext _context;
        public UserManager(WaPesDbContext context)
        {
            _context = context;
        }
        public async Task<User> AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<List<User>> AddMultipleAsync(List<User> usersToAdd)
        {
            await _context.Users.AddRangeAsync(usersToAdd);
            await _context.SaveChangesAsync();

            return usersToAdd;
        }

        public async Task<IReadOnlyCollection<User>> GetAllAsync()
        {
            return await _context.Users
                .Include(u => u.UserMembers)
                .Include(u => u.PlatformUsers)
                    .ThenInclude(up => up.Platform)
                .Include(u => u.UserMetadatas)
                    .ThenInclude(um => um.Metadata)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<User> UpdateAsync(User userToUpdate)
        {
            var currentUser = await _context.Users.FindAsync(userToUpdate.UserId);
            if (currentUser != null)
            {
                _context.Entry(currentUser).CurrentValues.SetValues(userToUpdate);
                await _context.SaveChangesAsync();
            }
            return currentUser;
        }

        public async Task<List<User>> UpdateMulitpleAsync(List<User> usersToUpdate)
        {
            _context.Users.UpdateRange(usersToUpdate);
            await _context.SaveChangesAsync();

            return usersToUpdate;
        }
    }
}
