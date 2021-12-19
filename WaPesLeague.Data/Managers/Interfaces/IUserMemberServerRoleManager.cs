using System.Collections.Generic;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Discord;

namespace WaPesLeague.Data.Managers.Interfaces
{
    public interface IUserMemberServerRoleManager
    {
        public Task<List<UserMemberServerRole>> AddMultipleAsync(List<UserMemberServerRole> userMemberServerRoles);
        public Task<bool> DeleteMultipleAsync(List<UserMemberServerRole> userMemberServerRoles);
    }
}
