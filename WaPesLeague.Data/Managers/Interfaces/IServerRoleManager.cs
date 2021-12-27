using System.Collections.Generic;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Discord;

namespace WaPesLeague.Data.Managers.Interfaces
{
    public interface IServerRoleManager
    {
        public Task<List<ServerRole>> GetAllAsync();
        public Task<ServerRole> GetServerRoleByDiscordRoleIdAndServerIdAsync(string discordRoleId, int serverId);
        public Task<ServerRole> AddAsync(ServerRole serverRole);
        public Task<List<ServerRole>> AddMultipleAsync(List<ServerRole> serverRoles);
        public Task<List<ServerRole>> UpdateMultipleAsync(List<ServerRole> serverRoles);
        public Task<ServerRole> UpdateAsync(ServerRole serverRole);
    }
}
