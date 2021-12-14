using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Discord;

namespace WaPesLeague.Data.Managers.Interfaces
{
    public interface IServerSnipingManager
    {
        public Task<ServerSniping> AddAsync(ServerSniping serverSniping);
        public Task<ServerSniping> UpdateAsync(ServerSniping serverSniping);
        public Task DeleteAsync(int serverSnipingId);
    }
}
