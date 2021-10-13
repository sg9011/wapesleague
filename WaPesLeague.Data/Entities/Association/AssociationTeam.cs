using System;
using System.Collections.Generic;
using WaPesLeague.Data.Entities.Association.Enums;
using WaPesLeague.Data.Entities.Match;

namespace WaPesLeague.Data.Entities.Association
{
    public class AssociationTeam
    {
        public int AssociationTeamId { get; set; }
        public int AssociationId { get; set; }
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public TeamType TeamType { get; set; }
        public DateTime DateCreated { get; set; }

        public virtual List<AssociationTeamPlayer> AssociationTeamPlayers { get; set; }
        public virtual Association Association { get; set; }
        public virtual List<MatchTeam> MatchTeams { get; set; }
    }
}
