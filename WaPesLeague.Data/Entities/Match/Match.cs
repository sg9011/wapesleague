using System;
using System.Collections.Generic;
using WaPesLeague.Data.Entities.Association;
using WaPesLeague.Data.Entities.Match.Enums;

namespace WaPesLeague.Data.Entities.Match
{
    public class Match
    {
        public int MatchId { get; set; }
        public Guid Guid { get; set; }
        public int DivisionGroupRoundId { get; set; }
        public int Order { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DatePlanned { get; set; }
        public DateTime? DatePlayed { get; set; }
        public MatchStatus MatchStatus { get; set; }

        public virtual List<MatchTeam> MatchTeams { get; set; }
        public virtual DivisionGroupRound DivisionGroupRound { get; set; }
    }
}
