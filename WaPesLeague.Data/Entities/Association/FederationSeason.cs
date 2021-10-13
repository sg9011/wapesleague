using System.Collections.Generic;

namespace WaPesLeague.Data.Entities.Federation
{
    //WaPes Legend Season 2
    public class FederationSeason
    {
        public int FederationSeasonId { get; set; }
        public int FederationId { get; set; }
        public int Edition { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }

        public virtual Federation Federation { get; set; }
        public virtual List<FederationSeasonCompetition> FederationSeasonCompetitions { get; set; }

    }
}
