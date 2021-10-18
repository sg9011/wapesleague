using System;
using System.Collections.Generic;
using System.Linq;

namespace WaPesLeague.Data.Entities.Mix
{
    public class MixTeam
    {
        public int MixTeamId { get; set; }
        public int MixSessionId { get; set; }
        public string Name { get; set; }
        public bool PositionsLocked { get; set; } // Team Registration --> If a team plays vs Mix
        public int? LockedTeamPlayerCount { get; set; } // should be metadata or something like that

        public virtual MixSession MixSession { get; set; }
        public List<MixPosition> Formation { get; set; }
        public virtual List<MixTeamTag> Tags { get; set; }
        public virtual List<MixTeamRoleOpening> MixTeamRoleOpenings { get; set; }

        public bool HasNameOrTag(string codeOrTag)
        {
            return string.Equals(codeOrTag, Name, StringComparison.InvariantCultureIgnoreCase)
                || (Tags?.Any(t => string.Equals(codeOrTag, t.Tag, StringComparison.InvariantCultureIgnoreCase)) ?? false);
        }

        public string GetCaptainName()
        {
            var roomOwnerMember = Formation
                .SelectMany(p => p.Reservations.Where(r => r.UserId == MixSession.RoomOwnerId && r.DateEnd == null))
                .FirstOrDefault()
                ?.User.UserMembers.FirstOrDefault(um => um.ServerId == MixSession.MixChannel.MixGroup.ServerId);
            return roomOwnerMember?.DiscordNickName
                ?? Formation
                    .SelectMany(p => p.Reservations.Where(r => r.IsCaptain && r.DateEnd == null))
                    .FirstOrDefault()
                    ?.User.UserMembers.FirstOrDefault(um => um.ServerId == MixSession.MixChannel.MixGroup.ServerId)?.DiscordNickName
                ?? string.Empty;
        }
    }
}
