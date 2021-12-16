using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.User;

namespace WaPesLeague.Data.Managers.Interfaces
{
    public interface IUserMemberManager
    {
        public Task<UserMember> GetUserMemberByDiscordUserIdAndServerIdAsync(string discordUserId, string discordServerId);
        public Task<int?> GetUserIdByDiscordUserIdAsync(string discordUserId);
        public Task<UserMember> GetUserMemberWithSnipersByUserIdAndServerIdAsync(int userId, int serverId, DateTime date);
        public Task<int?> GetUserMemberIdByUserIdAndServerIdAsync(int userId, int serverId);
        public Task<UserMember> AddAsync(UserMember userMember);
        public Task<List<UserMember>> AddMultipleAsync(List<UserMember> userMembers);
        public Task<UserMember> UpdateAsync(UserMember userMember);
        public Task<IReadOnlyCollection<UserMember>> GetUserMembersByUserIdsAndServerId(List<int> userIds, int serverId);
    }
}
