using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Mix;

namespace WaPesLeague.Data.Managers.Interfaces
{
    public interface IMixGroupManager
    {
        public Task<MixGroup> GetActiveMixGroupByDiscordServerAndChannelIdAsync(string serverId, string channelId);
        public Task<MixGroup> GetMixGroupByIdAsync(int mixSessionId);
        public Task<MixGroup> AddAsync(MixGroup mixGroup);
        public Task<bool> DeActivateMixGroupAsync(int mixGroup);
        public Task<bool> HasActiveMixChannelsAsync(int mixGroupId);
    }
}
