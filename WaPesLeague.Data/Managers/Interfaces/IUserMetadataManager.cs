using System.Collections.Generic;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.User;

namespace WaPesLeague.Data.Managers.Interfaces
{
    public interface IUserMetadataManager
    {
        public Task<List<UserMetadata>> AddMultipleAsync(List<UserMetadata> userMetadatasToAdd);
        public Task<List<UserMetadata>> UpdateMultipleAsync(List<UserMetadata> userMetadatasToUpdate);
    }
}
