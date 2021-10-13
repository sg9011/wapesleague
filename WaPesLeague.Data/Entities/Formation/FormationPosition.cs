namespace WaPesLeague.Data.Entities.Formation
{
    public class FormationPosition
    {
        public int FormationPositionId { get; set; }
        public int PositionId { get; set; }
        public int FormationId { get; set; }

        public virtual Position.Position Position{ get; set; }
        public virtual Formation Formation{ get; set; }
    }
}
