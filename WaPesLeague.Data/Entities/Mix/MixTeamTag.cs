namespace WaPesLeague.Data.Entities.Mix
{
    public class MixTeamTag
    {
        public int MixTeamTagId { get; set; }
        public int MixTeamId { get; set; }
        public string Tag { get; set; }

        public virtual MixTeam MixTeam { get; set; }
    }
}
