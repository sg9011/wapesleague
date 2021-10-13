using System.Collections.Generic;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Platform;

namespace WaPesLeague.Data.Managers.Interfaces
{
    public interface IPlatformManager
    {
        public Task<IReadOnlyCollection<Platform>> GetPlatformsAsync();
        public Task<Platform> GetPlatformWithPlatformUserIdsByNameAsync(string name, string discordServerId);
    }
}
