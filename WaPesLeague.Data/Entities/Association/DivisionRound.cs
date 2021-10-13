using System.Collections.Generic;

namespace WaPesLeague.Data.Entities.Association
{
    public class DivisionRound
    {
        public int DivisionRoundId { get; set; }
        public int DivisionId { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }

        public virtual Division Division { get; set; }
        public virtual List<DivisionGroup> DivisionGroups { get; set; }
    }
}
