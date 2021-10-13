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
    }
}
