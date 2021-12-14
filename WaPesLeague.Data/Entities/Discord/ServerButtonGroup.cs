using System.Collections.Generic;
using WaPesLeague.Data.Entities.Discord.Enums;

namespace WaPesLeague.Data.Entities.Discord
{
    public class ServerButtonGroup
    {
        public int ServerButtonGroupId { get; set; }
        public int ServerId { get; set; }
        public ButtonGroupType ButtonGroupType { get; set; }
        public decimal UseRate { get; set; } //1 is always 0.5 = 50% 0.1 is 10% etc

        public virtual Server Server { get; set; }
        public virtual List<ServerButton> Buttons { get; set; }
    }
}
