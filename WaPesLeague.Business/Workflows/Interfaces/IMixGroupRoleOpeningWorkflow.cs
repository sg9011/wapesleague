using System.Collections.Generic;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Mix;

namespace WaPesLeague.Business.Workflows.Interfaces
{
    public interface IMixGroupRoleOpeningWorkflow
    {
        public Task<List<MixGroupRoleOpening>> GetMixGroupRoleOpeningsFromCacheOrDbAsync();
    }
}
