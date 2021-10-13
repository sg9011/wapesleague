using System.Collections.Generic;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Position;

namespace WaPesLeague.Data.Managers.Interfaces
{
    public interface IPositionManager
    {
        public Task<IReadOnlyCollection<Position>> GetAllPositionsAsync(int? serverId);
        public Task<Position> GetPostionByTagOrCodeAsync(string positionTagOrCode, int? serverId);
        public Task<IReadOnlyCollection<Position>> GetAllPositionsWithTagsAsync(int? serverId);
    }
}
