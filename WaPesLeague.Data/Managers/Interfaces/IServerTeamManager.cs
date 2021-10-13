using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Discord;

namespace WaPesLeague.Data.Managers.Interfaces
{
    public interface IServerTeamManager
    {
        public Task<ServerTeam> UpdateAsync(ServerTeam serverTeam);
    }
}
