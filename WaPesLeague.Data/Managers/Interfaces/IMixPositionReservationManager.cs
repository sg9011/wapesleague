using System.Collections.Generic;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Mix;

namespace WaPesLeague.Data.Managers.Interfaces
{
    public interface IMixPositionReservationManager
    {
        public Task<MixPositionReservation> AddAsync(MixPositionReservation mixPositionReservation);
        public Task<MixPositionReservation> UpdateAsync(MixPositionReservation mixPositionReservation);

        public Task<MixPositionReservation> GetActiveMixPositionReservationByServerIdAndDiscordChannelIdAndUserIdAsync(int serverId, string discordChannelId, int userId);
        public Task<IReadOnlyCollection<MixPositionReservation>> GetAllNonActiveReservationWithPlayTimeByUserId(int userId);
        public Task<IReadOnlyCollection<MixPositionReservation>> GetAll();
        public Task<IReadOnlyCollection<MixPositionReservation>> GetAllReservationsForSessionsAsync(IEnumerable<int> mixSessionIds);
    }
}
