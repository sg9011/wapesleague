using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Discord;
using WaPesLeague.Data.Managers.Interfaces;

namespace WaPesLeague.Data.Managers
{
    public class ServerRoleManager : IServerRoleManager
    {
        private readonly WaPesDbContext _context;
        public ServerRoleManager(WaPesDbContext context)
        {
            _context = context;
        }

        public async Task<ServerRole> AddAsync(ServerRole serverRole)
        {
            await _context.ServerRoles.AddAsync(serverRole);
            await _context.SaveChangesAsync();

            return serverRole;
        }

        public async Task<ServerRole> GetServerRoleByDiscordRoleIdAndServerIdAsync(string discordRoleId, int serverId)
        {
            return await _context.ServerRoles
                .AsNoTracking()
                .FirstOrDefaultAsync(sr => sr.DiscordRoleId == discordRoleId && sr.ServerId == serverId);

        }

        public async Task<ServerRole> UpdateAsync(ServerRole serverRole)
        {
            var currentServerRole = await _context.ServerRoles.FindAsync(serverRole.ServerRoleId);
            if (currentServerRole != null)
            {
                _context.Entry(currentServerRole).CurrentValues.SetValues(serverRole);
                await _context.SaveChangesAsync();
            }
            return currentServerRole;
        }
    }
}
