using System;
using System.Collections.Generic;
using WaPesLeague.Data.Entities.Association.Enums;

namespace WaPesLeague.Data.Entities.Association
{
    //WaPesTeamFootball WaPesInternationalFootball
    public class Association
    {
        public int AssociationId { get; set; }
        public int AssociationTenantId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public DateTime DateCreated { get; set; }
        public TeamType DefaultTeamType { get; set; }

        public virtual AssociationTenant AssociationTenant { get; set; }
        public virtual List<AssociationTeam> AssociationTeams { get; set; }
        public virtual List<AssociationLeagueGroup> AssociationLeagueGroups { get; set; }
    }
}
