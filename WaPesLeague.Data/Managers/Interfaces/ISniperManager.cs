using System.Collections.Generic;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Discord;

namespace WaPesLeague.Data.Managers.Interfaces
{
    public interface ISniperManager
    {
        public Task<Sniper> AddAsync(Sniper sniper);
        public Task<List<Sniper>> GetAllActiveAsync();
    }
}
