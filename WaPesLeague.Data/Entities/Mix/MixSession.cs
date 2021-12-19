using System;
using System.Collections.Generic;
using WaPesLeague.Data.Entities.Discord;

namespace WaPesLeague.Data.Entities.Mix
{
    public class MixSession
    {
        public int MixSessionId { get; set; }
        public int MixChannelId { get; set; }
        public DateTime DateRegistrationOpening { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateToClose { get; set; }
        public DateTime? DateClosed { get; set; }
        public DateTime? DateStatsCalculated { get; set; }
        public DateTime? DateCreated { get; set; }
        public int CrashCount { get; set; }
        public int MatchCount { get; set; }
        public DateTime? DateLastUpdated { get; set; } //used to validate crashcount and matchcount

        public string GameRoomName { get; set; }
        public int? RoomOwnerId { get; set; }
        public string Server { get; set; }
        public string Password { get; set; }

        public virtual List<MixTeam> MixTeams { get; set; }
        public virtual MixChannel MixChannel { get; set; }
        public virtual User.User RoomOwner { get; set; }

        public virtual List<MixUserPositionSessionStat> UserPositionStats { get; set; }
        public virtual List<Sniper> Snipers { get; set; }
    }
}
