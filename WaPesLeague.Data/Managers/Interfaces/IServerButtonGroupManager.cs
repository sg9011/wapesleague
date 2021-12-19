using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Discord;

namespace WaPesLeague.Data.Managers.Interfaces
{
    public interface IServerButtonGroupManager
    {
        public Task<ServerButtonGroup> AddAsync(ServerButtonGroup serverButtonGroup);
        public Task<ServerButtonGroup> UpdateAsync(ServerButtonGroup serverButtonGroup);
    }
}
