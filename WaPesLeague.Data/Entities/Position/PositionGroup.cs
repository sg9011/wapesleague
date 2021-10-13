using System.Collections.Generic;

namespace WaPesLeague.Data.Entities.Position
{
    /// <summary>
    /// GoalKeepers, Defenders, Midfielders, Attackers
    /// </summary>
    public class PositionGroup
    {
        public int PositionGroupId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }

        public int? BaseValue { get; set; }// value to assign the cost of a position --> to try to avoid everyone taking the same position over and over again
        public virtual List<Position> Positions { get; set; }
    }
}
