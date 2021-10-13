using System;
using System.Collections.Generic;

namespace WaPesLeague.Data.Entities.Federation
{
    //WaPes Legend Season 2 DIV 2 Round 5
    public class FederationSeasonCompetitionRound
    {
        public int FederationSeasonCompetitionRoundId { get; set; }
        public Guid FederationSeasonCompetitionRoundGuid { get; set; }
        public int FederationSeasonCompetitionId { get; set; }
        public int Order { get; set; }
        public int Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        //Calculate Completed or Use Property Completed
        //We Can Also Add Social Media to this Entity --> MatchOfTheWeek Etc..

        public virtual List<Match.Match> Matches { get; set; }
        public virtual FederationSeasonCompetition FederationSeasonCompetition { get; set; }
        public virtual List<FederationSeasonCompetitionRoundSocialMedia> SocialMedias { get; set; }
    }
}
