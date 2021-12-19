using System.Collections.Generic;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.User;
using WaPesLeague.Data.Managers.Interfaces;

namespace WaPesLeague.Data.Managers
{
    public class UserMetadataManager : IUserMetadataManager
    {
        private readonly WaPesDbContext _context;
        public UserMetadataManager(WaPesDbContext context)
        {
            _context = context;
        }


        public async Task<List<UserMetadata>> AddMultipleAsync(List<UserMetadata> userMetadatasToAdd)
        {
            await _context.UserMetadatas.AddRangeAsync(userMetadatasToAdd);
            await _context.SaveChangesAsync();

            return userMetadatasToAdd;
        }

        public async Task<List<UserMetadata>> UpdateMultipleAsync(List<UserMetadata> userMetadatasToUpdate)
        {
            _context.UserMetadatas.UpdateRange(userMetadatasToUpdate);
            await _context.SaveChangesAsync();

            return userMetadatasToUpdate;
        }
    }
}
