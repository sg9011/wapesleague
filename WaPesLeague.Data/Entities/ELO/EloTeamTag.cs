namespace WaPesLeague.Data.Entities.ELO
{
    public class ELOTeamTag
    {
        public int EloTeamTagId { get; set; }
        public int EloTeamId { get; set; }
        public string Tag { get; set; }

        public virtual EloTeam EloTeam { get; set; }
    }
}
