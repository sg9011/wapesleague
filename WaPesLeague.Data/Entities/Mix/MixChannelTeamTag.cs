
namespace WaPesLeague.Data.Entities.Mix
{
    public class MixChannelTeamTag
    {
        public int MixChannelTeamTagId { get; set; }
        public int MixChannelTeamId { get; set; }
        public string Tag { get; set; }

        public virtual MixChannelTeam MixChannelTeam { get; set; }
    }
}
