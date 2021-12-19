using System;
using System.Collections.Generic;
using WaPesLeague.Data.Entities.Discord;
using WaPesLeague.Data.Entities.User;
using WaPesLeague.Data.Helpers;

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
        public Data.Entities.Discord.Server Server { get; set; }
        public List<string> RoleIds { get; set; }
        public List<string> ActorRoleIds { get; set; }
        public ulong RequestedByDiscordUserId { get; set; }
        public UserMember UserMember { get; set; }
        public DateTime DbSignInTime { get; set; }


        public SignInDto(ulong discordServerId, ulong discordChannelId, int userId, string team, string position, string extraInfo, Data.Entities.Discord.Server server, List<string> roleIds, List<string> actorRoleIds, ulong requestedByDiscordUserId, UserMember userMember)
        {
            DiscordServerId = discordServerId;
            DiscordChannelId = discordChannelId;
            UserId = userId;
            Team = team;
            Position = position;
            ExtraInfo = extraInfo;
            Server = server;
            RoleIds = roleIds;
            ActorRoleIds = actorRoleIds;
            RequestedByDiscordUserId = requestedByDiscordUserId;
            UserMember = userMember;
            DbSignInTime = DateTimeHelper.GetDatabaseNow();
        }
    }
}
