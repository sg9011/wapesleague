using System.Collections.Generic;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Formation;

namespace WaPesLeague.Data.Managers.Interfaces
{
    public interface IServerFormationManager
    {
        public Task<ServerFormation> AddAsync(ServerFormation serverFormation);
        public Task<ServerFormation> UpdateAsync(ServerFormation serverFormation);
        public Task<bool> HasFormationWithTagOrNameAsync(string tagOrName, string discordServerId);
        public Task<ServerFormation> GetDefaultFormationAsync(int serverId);
        public Task<ServerFormation> GetFormationByTagOrNameAsync(string tagOrName , int serverId);
        public Task<IReadOnlyCollection<ServerFormation>> GetAllFormationsByDiscordServerIdAsync(string discordServerId);
    }
}
