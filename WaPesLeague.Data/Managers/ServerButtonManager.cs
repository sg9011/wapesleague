using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Discord;
using WaPesLeague.Data.Helpers;
using WaPesLeague.Data.Managers.Interfaces;

namespace WaPesLeague.Data.Managers
{
    public class ServerButtonManager : IServerButtonManager
    {
        private readonly WaPesDbContext _context;
        public ServerButtonManager(WaPesDbContext context)
        {
            _context = context;
        }

        public async Task<ServerButton> AddAsync(ServerButton serverButton)
        {
            await _context.ServerButtons.AddAsync(serverButton);
            await _context.SaveChangesAsync();

            return serverButton;
        }

        public async Task<ServerButton> EndAsync(int serverButtonId)
        {
            var currentServerButton = await _context.ServerButtons.FindAsync(serverButtonId);
            if (currentServerButton != null)
            {
                currentServerButton.ShowUntil = DateTimeHelper.GetDatabaseNow();
                _context.Entry(currentServerButton).CurrentValues.SetValues(currentServerButton);
                await _context.SaveChangesAsync();
            }
            return currentServerButton;
        }
    }
}
