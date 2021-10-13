using System;
using System.Collections.Generic;

namespace WaPesLeague.Data.Entities.Mix
{
    public class MixPosition
    {
        public int MixPositionId { get; set; }
        public int MixTeamId { get; set; }
        public int PositionId { get; set; }

        public DateTime DateStart { get; set; }
        public DateTime? DateEnd { get; set; }

        public virtual MixTeam MixTeam { get; set; }
        public virtual Position.Position Position { get; set; }
        public virtual List<MixPositionReservation> Reservations { get; set; }
    }
}
