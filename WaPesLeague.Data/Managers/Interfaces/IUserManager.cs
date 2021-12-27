using System.Collections.Generic;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.User;

namespace WaPesLeague.Data.Managers.Interfaces
{
    public interface IUserManager
    {
        public Task<User> AddAsync(User user);
        public Task<IReadOnlyCollection<User>> GetAllAsync();
        public Task<List<User>> AddMultipleAsync(List<User> usersToAdd);
        public Task<User> UpdateAsync(User userToUpdate);
        public Task<List<User>> UpdateMulitpleAsync(List<User> usersToUpdate);
    }
}
