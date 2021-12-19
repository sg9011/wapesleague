using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Discord;
using WaPesLeague.Data.Managers.Interfaces;

namespace WaPesLeague.Data.Managers
{
    public class UserMemberServerRoleManager : IUserMemberServerRoleManager
    {
        private readonly WaPesDbContext _context;
        private readonly ILogger<UserMemberServerRoleManager> _logger;
        public UserMemberServerRoleManager(WaPesDbContext context, ILogger<UserMemberServerRoleManager> logger)
        {
            _context = context;
            _logger = logger;
        }


        public async Task<List<UserMemberServerRole>> AddMultipleAsync(List<UserMemberServerRole> userMemberServerRoles)
        {
            await _context.UserMemberServerRoles.AddRangeAsync(userMemberServerRoles);
            await _context.SaveChangesAsync();

            return userMemberServerRoles;
        }

        public async Task<bool> DeleteMultipleAsync(List<UserMemberServerRole> userMemberServerRoles)
        {
            try
            {
                _context.UserMemberServerRoles.RemoveRange(userMemberServerRoles);
                await _context.SaveChangesAsync();
                return true;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Error while deleting multiple userMemberServerRoles, records to delete: {userMemberServerRoles?.Count ?? 0}");
                return false;
            }
        }
    }
}
