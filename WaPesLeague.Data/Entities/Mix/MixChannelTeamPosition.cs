namespace WaPesLeague.Data.Entities.Mix
{
    public class MixChannelTeamPosition
    {
        public int MixChannelTeamPositionId { get; set; }
        public int MixChannelTeamId { get; set; }
        public int PositionId { get; set; }

        public virtual MixChannelTeam MixChannelTeam { get; set; }
        public virtual Position.Position Position { get; set; }
    }
}
