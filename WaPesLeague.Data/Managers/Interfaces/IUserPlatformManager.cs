using System.Collections.Generic;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.User;

namespace WaPesLeague.Data.Managers.Interfaces
{
    public interface IUserPlatformManager
    {
        public Task<IReadOnlyCollection<UserPlatform>> GetUserPlatformsByUserId(int userId);
        public Task<UserPlatform> GetUserPlatformByUserId(int userId, int platformId);
        public Task<UserPlatform> AddAsync(UserPlatform userPlatform);
        public Task<UserPlatform> UpdateAsync(UserPlatform userPlatform);
    }
}
