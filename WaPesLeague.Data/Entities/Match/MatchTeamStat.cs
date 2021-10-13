namespace WaPesLeague.Data.Entities.Match
{
    public class MatchTeamStat
    {
        public int MatchTeamStatId { get; set; }
        public int MatchTeamId { get; set; }
        public int MatchTeamStatTypeId { get; set; }
        public int Value { get; set; }

        public virtual MatchTeamStatType MatchTeamStatType { get; set; }
        public virtual MatchTeam MatchTeam { get; set; }
    }
}
