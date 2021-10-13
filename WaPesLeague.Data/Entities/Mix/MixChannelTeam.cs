using System.Collections.Generic;

namespace WaPesLeague.Data.Entities.Mix
{
    public class MixChannelTeam
    {
        public int MixChannelTeamId { get; set; }
        public int MixChannelId { get; set; }
        public string MixChannelTeamName { get; set; }
        public bool IsOpen { get; set; }
        public virtual List<MixChannelTeamPosition> DefaultFormation { get; set; }
        public virtual MixChannel MixChannel { get; set; }
        public virtual List<MixChannelTeamTag> Tags { get; set; }

    }
}
