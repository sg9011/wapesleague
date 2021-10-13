using System.Collections.Generic;
using System.Threading.Tasks;
using WaPesLeague.Business.Dto.Platform;

namespace WaPesLeague.Business.Workflows.Interfaces
{
    public interface IPlatformWorkflow
    {
        public Task<IReadOnlyCollection<SimplePlatformDto>> GetAllPlatformsAsync();
        public Task<PlatformWithUsersDto> GetPlatformWithUsersAsync(string name, ulong discordServerId);
    }
}
