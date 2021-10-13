using System.Collections.Generic;

namespace WaPesLeague.Data.Entities.Federation
{
    //WaPes Legend Season 2 DIV 2
    public class FederationSeasonCompetition
    {
        public int FederationSeasonCompetitionId { get; set; }
        public int CompetitionId { get; set; }
        public int FederationSeasonId { get; set; }
        public int Order { get; set; }
        public int NumberOfTeamsPromoted { get; set; }
        public int NumberOfTeamsRelegated { get; set; }
        public virtual List<FederationSeasonCompetitionRound> Rounds { get; set; }
        public virtual Competition Competition { get; set; }
        public virtual FederationSeason FederationSeason { get; set; }

        //public int Edition { get; set; } 
        //public string EditionName { get; set;}
        //public LeagueType LeagueType { get; set; }
        //public int NumberOfTeams { get; set; }
        //public virtual List<Team> Teams { get; set; }
    }
}
