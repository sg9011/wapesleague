using System;
using System.Collections.Generic;

namespace WaPesLeague.Data.Entities.Association
{
    public class AssociationTenantPlayer
    {
        public int AssociationTenantPlayerId { get; set; }
        public int UserId { get; set; }
        public int AssociationTenantId { get; set; }
        public DateTime DateCreated { get; set; }

        public virtual User.User User { get; set; }
        public virtual AssociationTenant AssociationTenant { get; set; }
        public virtual List<AssociationTeamPlayer> AssociationTeamPlayers { get; set; }
    }
}
