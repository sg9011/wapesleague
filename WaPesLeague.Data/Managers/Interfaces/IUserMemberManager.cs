﻿using System.Collections.Generic;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.User;

namespace WaPesLeague.Data.Managers.Interfaces
{
    public interface IUserMemberManager
    {
        public Task<UserMember> GetUserMemberByDiscordUserIdAndServerIdAsync(string discordUserId, string discordServerId);
        public Task<int?> GetUserIdByDiscordUserIdAsync(string discordUserId);
        public Task<UserMember> AddAsync(UserMember userMember);
        public Task<UserMember> UpdateAsync(UserMember userMember);
        public Task<IReadOnlyCollection<UserMember>> GetUserMembersByUserIdsAndServerId(List<int> userIds, int serverId);
    }
}
