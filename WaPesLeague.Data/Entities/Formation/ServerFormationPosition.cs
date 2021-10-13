namespace WaPesLeague.Data.Entities.Formation
{
    public class ServerFormationPosition
    {
        public int ServerFormationPositionId { get; set; }
        public int PositionId { get; set; }
        public int ServerFormationId { get; set; }

        public virtual Position.Position Position { get; set; }
        public virtual ServerFormation Formation { get; set; }
    }
}
