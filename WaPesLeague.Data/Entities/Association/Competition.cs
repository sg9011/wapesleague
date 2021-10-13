using System.Collections.Generic;

namespace WaPesLeague.Data.Entities.Federation
{
    //Wapes Legend
    public class Competition
    {
        public int CompetitionId { get; set; }
        public string Name { get; set; }
        public string ExtraInfo { get; set; }

        public virtual List<FederationSeasonCompetition> FederationSeasonCompetitions { get; set; }
    }
}
