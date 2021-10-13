using System;
using System.Collections.Generic;
using WaPesLeague.Data.Entities.Association.Enums;
using WaPesLeague.Data.Entities.Match;

namespace WaPesLeague.Data.Entities.Association
{
    //This is used as history table Unique UserId, DateStart, DateEnd
    public class AssociationTeamPlayer
    {
        public int AssociationTeamPlayerId { get; set; }
        public int AssociationTeamId { get; set; }
        public int AssociationTenantPlayerId { get; set; }
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }
        public TeamMemberType TeamMemberType { get; set; }


        public virtual AssociationTeam AssociationTeam { get; set; }
        public virtual AssociationTenantPlayer AssociationTenantPlayer { get; set; }
        public virtual List<MatchTeamPlayer> MatchTeamPlayers { get; set; }
    }
}
