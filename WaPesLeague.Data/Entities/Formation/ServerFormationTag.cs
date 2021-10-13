namespace WaPesLeague.Data.Entities.Formation
{
    public class ServerFormationTag
    {
        public int ServerFormationTagId { get; set; }
        public int ServerFormationId { get; set; }
        public string Tag { get; set; }

        public virtual ServerFormation Formation { get; set; }
    }
}
