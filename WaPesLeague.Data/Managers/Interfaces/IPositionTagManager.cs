using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Position;

namespace WaPesLeague.Data.Managers.Interfaces
{
    public interface IPositionTagManager
    {
        public Task<PositionTag> AddAsync(PositionTag positionTag);
        public Task<PositionTag> UpdateAsync(PositionTag positionTag);
        public Task DeleteAsync(int positionTagId);
    }
}
