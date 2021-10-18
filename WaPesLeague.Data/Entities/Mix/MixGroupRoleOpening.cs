using System;
using WaPesLeague.Data.Entities.Discord;

namespace WaPesLeague.Data.Entities.Mix
{
    public class MixGroupRoleOpening
    {
        public int MixGroupRoleOpeningId { get; set; }
        public int ServerRoleId { get; set; }
        public int MixGroupId { get; set; }
        public int Minutes { get; set; } //Negative number means early opening, positive means late opening

        public DateTime DateCreated { get; set; }
        public bool IsActive { get; set; }
        public DateTime? DateEnd { get; set; }


        public virtual ServerRole ServerRole { get; set; }
        public virtual MixGroup MixGroup { get; set; }
    }
}
