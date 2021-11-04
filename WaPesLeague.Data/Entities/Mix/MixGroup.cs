using System.Collections.Generic;
using WaPesLeague.Data.Entities.Discord;

namespace WaPesLeague.Data.Entities.Mix
{
    public class MixGroup
    {
        public int MixGroupId { get; set; }
        public string BaseChannelName { get; set; }
        public int ServerId { get; set; }
        public bool Recurring { get; set; }
        public bool CreateExtraMixChannels { get; set; }
        public string ExtraInfo { get; set; }
        public decimal Start { get; set; }
        public decimal HoursToOpenRegistrationBeforeStart { get; set; }
        public decimal MaxSessionDurationInHours { get; set; }
        public bool IsActive { get; set; }


        public virtual List<MixChannel> MixChannels { get; set; }
        public virtual List<MixGroupRoleOpening> MixGroupRoleOpenings { get; set; }
        public virtual Server Server { get; set; }
    }
}
