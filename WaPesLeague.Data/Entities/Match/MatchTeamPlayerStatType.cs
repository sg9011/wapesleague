using System.Collections.Generic;

namespace WaPesLeague.Data.Entities.Match
{
    public class MatchTeamPlayerStatType
    {
        public int MatchTeamPlayerStatTypeId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }

        public virtual List<MatchTeamPlayerStat> MatchTeamPlayerStats { get; set; }

        //Komnami Rating/Note

        //Goals
        //Assists

        //Interceptions
        //Touches

        //Tackles Attempted
        //Tackles Completed

        //Passes Attempted
        //Passes Completed
        //Pass Percentage

        //Shots Attempted
        //Shots Completed
        //Shots Percentage

        //Fouls
        //Offsides
    }
}
