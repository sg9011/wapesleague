using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Discord;

namespace WaPesLeague.Business.Workflows.Interfaces
{
    public interface IServerRoleWorkflow
    {
        public Task<ServerRole> GetOrCreateServerRoleByDiscordRoleIdAndServerAsync(ulong discordRoleId, string roleName, int serverId);
    }
}
