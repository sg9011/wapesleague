
namespace WaPesLeague.Data.Entities.Discord
{
    public class ServerTeamTag
    {
        public int ServerTeamTagId { get; set; }
        public int ServerTeamId { get; set; }
        public string Tag { get; set; }

        public virtual ServerTeam Team { get; set; }
    }
}
