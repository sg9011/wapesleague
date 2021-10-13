namespace WaPesLeague.Data.Entities.Match
{
    public class MatchTeamPlayerStat
    {
        public int MatchTeamPlayerStatId { get; set; }
        public int MatchTeamPlayerId { get; set; }
        public int MatchPlayerStatTypeId { get; set; }
        public int Value { get; set; }

        public virtual MatchTeamPlayerStatType MatchPlayerStatType { get; set; }
        public virtual MatchTeamPlayer MatchTeamPlayer { get; set; }
    }
}
