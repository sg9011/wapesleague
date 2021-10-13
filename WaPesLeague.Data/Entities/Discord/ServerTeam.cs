using System.Collections.Generic;

namespace WaPesLeague.Data.Entities.Discord
{
    public class ServerTeam
    {
        public int ServerTeamId { get; set; }
        public int ServerId { get; set; }
        public string Name { get; set; }
        public bool IsOpen { get; set; }
        public virtual Server Server { get; set; }

        public virtual List<ServerTeamTag> Tags { get; set; }
    }
}
