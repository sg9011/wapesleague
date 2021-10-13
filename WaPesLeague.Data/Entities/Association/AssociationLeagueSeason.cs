using System;
using System.Collections.Generic;

namespace WaPesLeague.Data.Entities.Association
{
    public class AssociationLeagueSeason
    {
        public int AssociationLeagueSeasonId { get; set; }
        public int AssociationLeagueGroupId { get; set; }
        public string Name { get; set; }
        public string Edition { get; set; }
        public int Order { get; set; }
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }

        public virtual AssociationLeagueGroup AssociationLeagueGroup { get; set; }
        public virtual List<Division> Divisions { get; set; }
    }
}
