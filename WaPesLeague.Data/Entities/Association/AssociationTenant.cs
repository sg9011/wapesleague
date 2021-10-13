using System.Collections.Generic;

namespace WaPesLeague.Data.Entities.Association
{
    //WaPes Or Fifa
    public class AssociationTenant
    {
        public int AssociationTenantId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Website { get; set; }

        public virtual List<Association> Associations { get; set; }
        public virtual List<AssociationTenantPlayer> AssociationTenantPlayers { get; set; }
    }
}
