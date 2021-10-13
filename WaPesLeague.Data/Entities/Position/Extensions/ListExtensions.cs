using System;
using System.Collections.Generic;
using System.Linq;

namespace WaPesLeague.Data.Entities.Position.Extensions
{
    public static class ListExtensions
    {
        public static int GetPositionIdByCode(this IEnumerable<Position> positions, string code)
        {
            return positions.Single(p => string.Equals(p.Code, code, StringComparison.InvariantCultureIgnoreCase)).PositionId;
        }

        public static int GetPositionGroupIdByCode(this IEnumerable<PositionGroup> positionGroups, string code)
        {
            return positionGroups.Single(pg => string.Equals(pg.Code, code, StringComparison.InvariantCultureIgnoreCase)).PositionGroupId;
        }
    }
}
