using System;
using System.Collections.Generic;
using WaPesLeague.Data.Entities.Association;

namespace WaPesLeague.Data.Entities.Match
{
    public class MatchTeam
    {
        public int MatchTeamId { get; set; }
        public int MatchId { get; set; }
        public int TeamId { get; set; }
        public int? Goals { get; set; }
        public DateTime? DateConfirmed { get; set; }
        public int? ConfirmedById { get; set; }

        public virtual AssociationTeam Team { get; set; }
        public virtual List<MatchTeamPlayer> MatchTeamPlayers { get; set; }
        public virtual List<MatchTeamStat> MatchTeamStats { get; set; }
        public virtual Match Match { get; set; }
        public virtual User.User ConfirmedBy { get; set; }
    }
}
