using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Mix;
using WaPesLeague.Data.Helpers;

namespace WaPesLeague.Data.Managers.Interfaces
{
    public interface IMixSessionManager
    {
        public Task<MixSession> GetCurrentMixSessionByChannelIdAsync(int mixChannelId);
        public Task<MixSession> GetMixSessionByIdAsync(int mixSessionId, int serverId);
        public Task<MixSession> GetActiveMixSessionByServerIdAndDiscordChannelIdAsync(int serverId, string discordchannelId);
        public Task<bool> HasOpenMixSessionByDiscordIds(string discordServerid, string disordChannelId, DateTime time);
        public Task<bool> CheckIfExtraMixSessionShouldBeCreatedAsync(int mixGroupId);
        public Task<MixSession> AddAsync(MixSession mixSession);
        public Task<MixSession> UpdateAsync(MixSession mixSession);
        //public Task<MixSession> SetFollowUpSessionCreatedAsync(int mixSessionId, bool followUpSessionCreated);
        public Task<IReadOnlyCollection<MixSession>> GetMixSessionsToCloseAsync();
        public Task<EndDbSessionResult> EndCurrentSessionAsync(int mixSessionId);
        public Task<IReadOnlyCollection<MixSession>> GetActiveMixSessionsByMixGroupIdAsync(int mixGroupId);
        public Task<IReadOnlyCollection<MixSession>> GetMixSessionsThatWillStartBetweenDatesAsync(DateTime rangeStartTime, DateTime rangeEndTime);
        public Task<IReadOnlyCollection<MixSession>> GetMixSessionAndTeamsBySessionIdsAsync(IEnumerable<int> mixSessionIds);

        public Task<IReadOnlyCollection<MixSession>> GetSessionsToCalculateStatsAsync();
        public Task SetStatsCalculatedDateForMixSessionsAsync(IEnumerable<int> mixSessionIds);
        public Task<Dictionary<int, int>> GetAllSessionIdsWithServerId();
    }
}
