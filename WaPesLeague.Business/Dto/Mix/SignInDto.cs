using System.Collections.Generic;

namespace WaPesLeague.Business.Dto.Mix
{
    public class SignInDto
    {
        public ulong DiscordServerId { get; set; }
        public ulong DiscordChannelId { get; set; }
        public int UserId { get; set; }
        public string Team { get; set; }
        public string Position { get; set; }
        public string ExtraInfo { get; set; }
        public int ServerId { get; set; }
        public List<string> RoleIds { get; set; }


        public SignInDto(ulong discordServerId, ulong discordChannelId, int userId, string team, string position, string extraInfo, int serverId, List<string> roleIds)
        {
            DiscordServerId = discordServerId;
            DiscordChannelId = discordChannelId;
            UserId = userId;
            Team = team;
            Position = position;
            ExtraInfo = extraInfo;
            ServerId = serverId;
            RoleIds = roleIds;
        }
    }
}
