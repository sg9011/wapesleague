using System.Collections.Generic;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Discord;

namespace WaPesLeague.Data.Managers.Interfaces
{
    public interface IServerManager
    {
        public Task<Server> AddAsync(Server server);
        public Task<Server> UpdateAsync(Server server);
        public Task<Server> GetServerByDiscordIdAsync(string discordServerId);
        public Task<IReadOnlyCollection<Server>> GetServersAsync();
        public Task<int?> GetServerIdByDiscordServerId(ulong discordServerId);
        public Task<bool> DeActivateServerAsync(int server);
    }
}
