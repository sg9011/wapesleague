using System.Collections.Generic;
using System.Linq;

namespace WaPesLeague.Business.Dto.Formation
{
    public class AddServerFormationDto
    {
        public string FormationName { get; set; }
        public ulong DiscordServerId { get; set; }
        public string DiscordServerName { get; set; }
        public List<string> Positions { get; set; }
        public int ServerId { get; set; }

        public AddServerFormationDto(string formationName, string positions, ulong discordServerId, string discordServerName, int serverId)
        {
            FormationName = formationName;
            DiscordServerId = discordServerId;
            DiscordServerName = discordServerName;
            Positions = positions?.Replace(" ", ",").Split(",", System.StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>();
            if ((Positions?.Any() ?? false) && Positions.Count == 10)
            {
                Positions.Add("GK");
            }
            ServerId = serverId;
        }
    }
}
