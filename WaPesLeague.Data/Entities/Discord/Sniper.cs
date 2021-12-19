using System;
using WaPesLeague.Data.Entities.Mix;
using WaPesLeague.Data.Entities.User;

namespace WaPesLeague.Data.Entities.Discord
{
    public class Sniper
    {
        public int SniperId { get; set; }
        public int UserMemberId { get; set; }
        public int InitiatedByServerSnipingId { get; set; }
        public int CatchedOnMixSessionId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateEnd { get; set; }

        public virtual UserMember UserMember { get; set; }
        public virtual ServerSniping InitiatedByServerSniping { get; set; }
        public virtual MixSession CatchedOnMixSession { get; set; }

    }
}
