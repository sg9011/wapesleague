using System.Collections.Generic;

namespace WaPesLeague.Data.Entities.Match
{
    public class MatchTeamStatType
    {
        public int MatchTeamStatTypeId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }

        public virtual List<MatchTeamStat> MatchTeamStats { get; set; }

        //Possession,
        //FirstThirdPossession,
        //MidfieldPossession,
        //FinalThirdPossession,

        //Shots,
        //ShotsOnTarget,
        //ShotsOnTargetPercentage,

        //Passes,
        //PassesCompleted,
        //PassesCompletedPercentage,

        //LeftAttackingSidePercentage,
        //MiddleAttackingSidePercentage,
        //RightAttackingSidePercentage,

        //Interceptions,
        //Fouls,
        //Offside
    }
}
