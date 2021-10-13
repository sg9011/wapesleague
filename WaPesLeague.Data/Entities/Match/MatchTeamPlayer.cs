using System.Collections.Generic;
using WaPesLeague.Data.Entities.Association;

namespace WaPesLeague.Data.Entities.Match
{
    public class MatchTeamPlayer
    {
        public int MatchTeamPlayerId { get; set; }
        public int MatchTeamId { get; set; }
        public int? AssociationTeamPlayerId { get; set; }
        public string PlayerName { get; set; }
        public int PositionId { get; set; }
        public int? JerseyNumber { get; set; } //This is unique per team so we can have multiple players with the same name that are not known

        public virtual MatchTeam MatchTeam { get; set; }
        public virtual AssociationTeamPlayer AssociationTeamPlayer { get; set; }
        public virtual Position.Position Position { get; set; }
        public virtual List<MatchTeamPlayerStat> MatchTeamPlayerStats { get; set; }
        public virtual List<MatchTeamPlayerEvent> MatchTeamPlayerEvents { get; set; }
    }
}
