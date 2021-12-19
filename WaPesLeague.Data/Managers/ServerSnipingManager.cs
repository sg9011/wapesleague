using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Discord;
using WaPesLeague.Data.Managers.Interfaces;

namespace WaPesLeague.Data.Managers
{
    public class ServerSnipingManager : IServerSnipingManager
    {
        private readonly WaPesDbContext _context;
        public ServerSnipingManager(WaPesDbContext context)
        {
            _context = context;
        }

        public async Task<ServerSniping> AddAsync(ServerSniping serverSniping)
        {
            await _context.ServerSnipings.AddAsync(serverSniping);
            await _context.SaveChangesAsync();

            return serverSniping;
        }

        public async Task<ServerSniping> UpdateAsync(ServerSniping serverSniping)
        {
            var currentServerSniping = await _context.ServerSnipings.FindAsync(serverSniping.ServerSnipingId);
            if (currentServerSniping != null)
            {
                _context.Entry(currentServerSniping).CurrentValues.SetValues(serverSniping);
                await _context.SaveChangesAsync();
            }
            return currentServerSniping;
        }

        public async Task DeActivateAsync(int serverSnipingId)
        {
            var currentServerSniping = await _context.ServerSnipings.FindAsync(serverSnipingId);
            if (currentServerSniping != null)
            {
                currentServerSniping.Enabled = false;
                await _context.SaveChangesAsync();
            }
        }
    }
}
