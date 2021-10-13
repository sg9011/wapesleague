using System.Collections.Generic;

namespace WaPesLeague.Data.Entities.Association
{
    public class Division
    {
        public int DivisionId { get; set; }
        public int AssociationLeagueSeasonId { get; set; }
        public int Order { get; set; }

        public virtual AssociationLeagueSeason AssociationLeagueSeason { get; set; }
        public virtual List<DivisionRound> DivisionRounds { get; set; }
    }
}
