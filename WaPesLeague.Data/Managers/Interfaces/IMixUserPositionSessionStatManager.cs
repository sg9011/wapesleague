using System.Collections.Generic;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Mix;

namespace WaPesLeague.Data.Managers.Interfaces
{
    public interface IMixUserPositionSessionStatManager
    {
        public Task<List<MixUserPositionSessionStat>> AddMultipleAsync(List<MixUserPositionSessionStat> mixUserPositionSessionStats);
        public Task<List<MixUserPositionSessionStat>> GetAllAsync();
    }
}
