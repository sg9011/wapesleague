using System;
using System.Diagnostics;

namespace WaPesLeague.Business.Dto.Mix
{
    [DebuggerDisplay("{ReservationId}", Name = "MixPositionReservationTimeCalcDto")]
    public class MixPositionReservationTimeCalcDto
    {
        public int ReservationId { get; set; }
        public int MixSessionId { get; set; }
        public int UserId { get; set; }
        public int PositionId { get; set; }
        //public int? ParentPositionId { get; set; }
        public string PositionGroup { get; set; }
        public int PositionGroupOrder { get; set; }
        public DateTime CalculatedStartTime { get; set; }
        public DateTime CaluclatedEndTime { get; set; }
    }
}
