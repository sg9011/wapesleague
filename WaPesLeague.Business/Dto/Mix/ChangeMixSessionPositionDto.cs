namespace WaPesLeague.Business.Dto.Mix
{
    public class ChangeMixSessionPositionDto
    {
        public ulong DiscordServerId { get; set; }
        public ulong DiscordChannelId { get; set; }
        public int UserId { get; set; }
        public string Team { get; set; }
        public string OldPosition { get; set; }
        public string NewPosition { get; set; }
        public int ServerId { get; set; }

        public ChangeMixSessionPositionDto(ulong discordServerId, ulong discordChannelId, int userId, string team, string oldPosition, string newPosition, int serverId)
        {
            DiscordServerId = discordServerId;
            DiscordChannelId = discordChannelId;
            UserId = userId;
            Team = team;
            OldPosition = oldPosition;
            NewPosition = newPosition;
            ServerId = serverId;
        }
    }
}
