using System.Collections.Generic;

namespace WaPesLeague.Data.Entities.Mix
{
    /// <summary>
    /// Mix Channel is the Discord channel in which the bot is operating;
    /// </summary>
    public class MixChannel
    {
        public int MixChannelId { get; set; }
        public int MixGroupId { get; set; }
        public string DiscordChannelId { get; set; }
        public string ChannelName { get; set; }
        public int Order { get; set; }
        public bool Enabled { get; set; } //Set To false when discord channel is not foud

        public virtual List<MixChannelTeam> MixChannelTeams { get; set; }
        public virtual List<MixSession> MixSessions { get; set; }
        public virtual MixGroup MixGroup { get; set; }

        public static int StartOrderNumber => 1;
    }
}
