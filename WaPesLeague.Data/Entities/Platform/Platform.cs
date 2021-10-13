using System.Collections.Generic;
using WaPesLeague.Data.Entities.User;

namespace WaPesLeague.Data.Entities.Platform
{
    //PSN, Xbox, Steam ...
    public class Platform
    {
        public int PlatformId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual List<UserPlatform> PlatformUsers { get; set; }
    }
}
