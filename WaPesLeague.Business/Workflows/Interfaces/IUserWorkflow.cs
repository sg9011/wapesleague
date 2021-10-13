using System.Threading.Tasks;
using WaPesLeague.Business.Dto.User;
using WaPesLeague.Business.Dto;

namespace WaPesLeague.Business.Workflows.Interfaces
{
    public interface IUserWorkflow
    {
        public Task<int> GetOrCreateUserIdByDiscordId(DiscordCommandPropsDto propsDto);
        public Task<UserPlatformsDto> GetUserPlatformsByUserIdAsync(int userId);
        public Task<string> SetUserPlatformAsync(int userId, string platformName, string userName);
    }
}
