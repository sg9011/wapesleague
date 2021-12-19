using System;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Discord;

namespace WaPesLeague.Data.Managers.Interfaces
{
    public interface IServerButtonManager
    {
        public Task<ServerButton> AddAsync(ServerButton serverButton);
        public Task<ServerButton> EndAsync(int serverButtonId);
    }
}
