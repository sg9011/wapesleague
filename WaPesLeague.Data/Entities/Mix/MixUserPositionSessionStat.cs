using System;

namespace WaPesLeague.Data.Entities.Mix
{
    public class MixUserPositionSessionStat
    {
        public int MixUserPositionSessionStatId { get; set; }
        public int UserId { get; set; }
        public int PositionId { get; set; }
        public int MixSessionId { get; set; }
        public int PlayTimeSeconds { get; set; }


        public virtual Position.Position Position { get; set; }
        public virtual MixSession MixSession { get; set; }
        public virtual User.User User { get; set; }
    }
}
