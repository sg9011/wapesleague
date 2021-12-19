using System.Collections.Generic;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Metadata;

namespace WaPesLeague.Data.Managers.Interfaces
{
    public interface IMetadataManager
    {
        public Task<Metadata> AddAsync(Metadata metadata);
        public Task<List<Metadata>> GetAllAsync();
    }
}
