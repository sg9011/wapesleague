using System.Collections.Generic;
using WaPesLeague.Data.Entities.Discord;

namespace WaPesLeague.Data.Entities.Formation
{
    public class ServerFormation
    {
        public int ServerFormationId { get; set; }
        public int ServerId { get; set; }
        public string Name { get; set; }
        public bool IsDefault { get; set; }

        public virtual List<ServerFormationTag> Tags { get; set; }
        public virtual List<ServerFormationPosition> Positions { get; set; }
        public virtual Server Server { get; set; }
    }
}
