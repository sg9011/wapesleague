using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.User;
using WaPesLeague.Data.Managers.Interfaces;

namespace WaPesLeague.Data.Managers
{
    public class UserMemberManager : IUserMemberManager
    {
        private readonly WaPesDbContext _context;
        public UserMemberManager(WaPesDbContext context)
        {
            _context = context;
        }

        public async Task<UserMember> GetUserMemberByDiscordUserIdAndServerIdAsync(string discordUserId, string serverId)
        {
            return await _context.UserMembers
                .AsNoTracking()
                .FirstOrDefaultAsync(um => um.DiscordUserId == discordUserId && um.Server.DiscordServerId == serverId);
        }

        public async Task<int?> GetUserIdByDiscordUserIdAsync(string discordUserId)
        {
            return (await _context.UserMembers
                .AsNoTracking()
                .FirstOrDefaultAsync(um => um.DiscordUserId == discordUserId))?.UserId;
        }

        public async Task<UserMember> AddAsync(UserMember userMember)
        {
            await _context.UserMembers.AddAsync(userMember);
            await _context.SaveChangesAsync();

            return userMember;
        }

        public async Task<UserMember> UpdateAsync(UserMember userMember)
        {
            var currentUserMember = await _context.UserMembers.FindAsync(userMember.UserMemberId);
            if (currentUserMember != null)
            {
                _context.Entry(currentUserMember).CurrentValues.SetValues(userMember);
                await _context.SaveChangesAsync();
            }
            return currentUserMember;
        }

        public async Task<IReadOnlyCollection<UserMember>> GetUserMembersByUserIdsAndServerId(List<int> userIds, int serverId)
        {
            if (userIds != null && userIds.Any())
            {
                return await _context.UserMembers
                    .Where(um => userIds.Any(ui => ui == um.UserId) && um.ServerId == serverId)
                    .AsNoTracking()
                    .ToListAsync();
            }
            return new List<UserMember>();
        }
    }
}
