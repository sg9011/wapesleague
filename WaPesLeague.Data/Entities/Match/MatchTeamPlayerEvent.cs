using WaPesLeague.Data.Entities.Match.Enums;

namespace WaPesLeague.Data.Entities.Match
{
    public class MatchTeamPlayerEvent
    {
        public int MatchTeamPlayerEventId { get; set; }
        public int MatchTeamPlayerId { get; set; }
        public MatchEventType Event { get; set; }
        public int? Minute { get; set; } // start for each period from minute 0?
        public Period? Period { get; set; }

        public virtual MatchTeamPlayer MatchTeamPlayer { get; set; }

    }
}
