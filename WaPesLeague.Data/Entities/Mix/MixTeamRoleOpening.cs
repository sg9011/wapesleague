using System;
using WaPesLeague.Data.Entities.Discord;

namespace WaPesLeague.Data.Entities.Mix
{
    public class MixTeamRoleOpening
    {
        public int MixTeamRoleOpeningId { get; set; }
        public int ServerRoleId { get; set; }
        public int MixTeamId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; } 


        public virtual ServerRole ServerRole { get; set; }
        public virtual MixTeam MixTeam { get; set; }
    }
}
