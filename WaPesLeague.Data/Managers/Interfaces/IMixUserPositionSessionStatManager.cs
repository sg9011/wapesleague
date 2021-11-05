using System.Collections.Generic;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Mix;
using WaPesLeague.Data.Entities.User;
using WaPesLeague.Data.Helpers;

namespace WaPesLeague.Data.Managers.Interfaces
{
    public interface IMixUserPositionSessionStatManager
    {
        public Task<List<MixUserPositionSessionStat>> AddMultipleAsync(List<MixUserPositionSessionStat> mixUserPositionSessionStats);
        public Task<List<MixUserPositionSessionStat>> GetAllAsync();
        public Task<List<UserIdAndSessionAmount>> GetUserIdsWithSessionAmountAsync(int serverId);
    }
}
