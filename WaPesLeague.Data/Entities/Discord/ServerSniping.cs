using System.Collections.Generic;

namespace WaPesLeague.Data.Entities.Discord
{
    public class ServerSniping
    {
        public int ServerSnipingId { get; set; }
        public int ServerId { get; set;  }
        public int IntervalAfterRegistrationOpeningInMinutes { get; set; }
        public int SignUpDelayInMinutes { get; set; }
        public int SignUpDelayDurationInHours { get; set; }
        public bool Enabled { get; set; }

        public virtual Server Server { get; set; }
        public virtual List<Sniper> Snipers { get; set; }
    }
}