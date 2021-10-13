using System.Collections.Generic;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Mix;

namespace WaPesLeague.Data.Managers.Interfaces
{
    public interface IMixPositionManager
    {
        public Task<IReadOnlyCollection<MixPosition>> GetAvailablePositionsForTeamAsync(int mixTeamId);
        public Task<MixPosition> GetActivePositionByUserIdMixTeamIdAsync(int mixTeamId, int userId);
        public Task<MixPosition> AddAsync(MixPosition mixPosition);
        public Task<List<MixPosition>> AddMultipleAsync(List<MixPosition> mixPositions);
        public Task<MixPosition> UpdateAsync(MixPosition mixPosition);
        public Task<IReadOnlyCollection<MixPosition>> GetActivePositionsByDiscordIdsAndTeamAndPositionAsync(string discordServerId, string discordChannelId, string team, string oldPosition);
    }
}
