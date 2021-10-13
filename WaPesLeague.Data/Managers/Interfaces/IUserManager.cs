using System.Threading.Tasks;
using WaPesLeague.Data.Entities.User;

namespace WaPesLeague.Data.Managers.Interfaces
{
    public interface IUserManager
    {
        public Task<User> AddAsync(User user);
    }
}
