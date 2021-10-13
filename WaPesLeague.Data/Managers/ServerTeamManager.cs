using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Discord;
using WaPesLeague.Data.Managers.Interfaces;

namespace WaPesLeague.Data.Managers
{
    public class ServerTeamManager : IServerTeamManager
    {
        private readonly WaPesDbContext _context;
        public ServerTeamManager(WaPesDbContext context)
        {
            _context = context;
        }
        public async Task<ServerTeam> UpdateAsync(ServerTeam ServerTeam)
        {
            var currentServerTeam = await _context.ServerTeams.FindAsync(ServerTeam.ServerTeamId);
            if (currentServerTeam != null)
            {
                _context.Entry(currentServerTeam).CurrentValues.SetValues(ServerTeam);
                await _context.SaveChangesAsync();
            }
            return currentServerTeam;
        }
    }
}
