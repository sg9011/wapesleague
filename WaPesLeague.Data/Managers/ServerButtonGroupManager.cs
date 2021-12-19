using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Discord;
using WaPesLeague.Data.Managers.Interfaces;

namespace WaPesLeague.Data.Managers
{
    public class ServerButtonGroupManager : IServerButtonGroupManager
    {
        private readonly WaPesDbContext _context;
        public ServerButtonGroupManager(WaPesDbContext context)
        {
            _context = context;
        }

        public async Task<ServerButtonGroup> AddAsync(ServerButtonGroup serverButtonGroup)
        {
            await _context.ServerButtonGroups.AddAsync(serverButtonGroup);
            await _context.SaveChangesAsync();

            return serverButtonGroup;
        }

        public async Task<ServerButtonGroup> UpdateAsync(ServerButtonGroup serverButtonGroup)
        {
            var currentServerButtonGroup = await _context.ServerButtonGroups.FindAsync(serverButtonGroup.ServerButtonGroupId);
            if (currentServerButtonGroup != null)
            {
                _context.Entry(currentServerButtonGroup).CurrentValues.SetValues(serverButtonGroup);
                await _context.SaveChangesAsync();
            }
            return currentServerButtonGroup;
        }
    }
}
