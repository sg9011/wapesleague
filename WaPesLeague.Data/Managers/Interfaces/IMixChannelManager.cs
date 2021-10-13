using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Mix;

namespace WaPesLeague.Data.Managers.Interfaces
{
    public interface IMixChannelManager
    {
        public Task<int?> GetActiveMixChannelIdByDiscordIds(string discordServerId, string discordChannelId);
        public Task<MixChannel> GetActiveMixChannelByDiscordIds(string discordServerId, string discordChannelId);
        public Task<MixChannel> GetActiveMixChannelByDiscordChannelIdAndInternalServerId(int serverId, string discordChannelId);
        public Task<bool> HasActiveMixChannelByDiscordIdsAsync(string discordServerId, string discordChannelId);
        public Task<bool> HasActiveMixChannelByDiscordIdsAndTeamAndPositionAsync(string discordServerId, string discordChannelId, string team, string position);
        public Task<bool> UserCanSignIntoMixChannelAsync(string discordServerId, string discordChannelId, int userId);
        public Task<MixChannel> AddAsync(MixChannel mixChannel);
        public Task<MixChannel> UpdateAsync(MixChannel mixChannel);
        public Task<bool> DisableChannelAsync(int mixChannelId);
    }
}
