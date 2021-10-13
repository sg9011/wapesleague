using System.Collections.Generic;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Formation;

namespace WaPesLeague.Data.Managers.Interfaces
{
    public interface IFormationManager
    {
        public Task<IReadOnlyCollection<Formation>> GetAllFormationsAsync();
    }
}
