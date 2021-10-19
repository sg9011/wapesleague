using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Discord;

namespace WaPesLeague.Data.Managers.Interfaces
{
    public interface IServerRoleManager
    {
        public Task<ServerRole> GetServerRoleByDiscordRoleIdAndServerIdAsync(string discordRoleId, int serverId);
        public Task<ServerRole> AddAsync(ServerRole serverRole);
        public Task<ServerRole> UpdateAsync(ServerRole serverRole);
    }
}
