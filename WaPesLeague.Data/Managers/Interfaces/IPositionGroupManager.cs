using System.Collections.Generic;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Position;

namespace WaPesLeague.Data.Managers.Interfaces
{
    public interface IPositionGroupManager
    {
        public Task<IReadOnlyCollection<PositionGroup>> GetAllAsync();
    }
}
