using System.Collections.Generic;
using WaPesLeague.Data.Entities.Mix;

namespace WaPesLeague.Data.Entities.Discord
{
    public class ServerRole
    {
        public int ServerRoleId { get; set; }
        public int ServerId { get; set; }
        public string DiscordRoleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        

        public virtual Server Server { get; set; }
        public virtual List<MixGroupRoleOpening> MixGroupRoleOpenings { get; set; }
        public virtual List<MixTeamRoleOpening> MixTeamRoleOpenings { get; set; }
    }
}
