using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Base.Bot.Queue.Dto
{
    public class GuildMemberUpdatedDto
    {
        public ulong DiscordServerId { get; set; }
        public string DiscordServerName { get; set; }
        public ulong DiscordUserId { get; set; }

        public string NickNameBefore { get; set; }
        public string NickNameAfter { get; set; }

        public IReadOnlyCollection<DiscordRole> RolesBefore { get; set; }
        public IReadOnlyCollection<DiscordRole> RolesAfter { get; set; }
        public bool NickNameChanged => !string.Equals(NickNameBefore, NickNameAfter, StringComparison.InvariantCulture);

        public bool RolesChanged => ((RolesBefore?.Count ?? 0) != (RolesAfter?.Count ?? 0))
            || !(
                RolesBefore?.All(rb => 
                    RolesAfter?.Any(ra => ra.Id == rb.Id) ?? false
                )
                ?? false);

        public GuildMemberUpdatedDto(ulong discordServerId, string serverName, ulong discordUserId, string nickNameBefore, string nickNameAfter, IReadOnlyCollection<DiscordRole> rolesBefore, IReadOnlyCollection<DiscordRole> rolesAfter)
        {
            DiscordServerId = discordServerId;
            DiscordServerName = serverName;
            DiscordUserId = discordUserId;
            NickNameBefore = nickNameBefore;
            NickNameAfter = nickNameAfter;
            RolesBefore = rolesBefore;
            RolesAfter = rolesAfter;
        }
    }
}
