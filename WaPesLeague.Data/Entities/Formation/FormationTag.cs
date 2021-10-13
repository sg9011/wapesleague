namespace WaPesLeague.Data.Entities.Formation
{
    public class FormationTag
    {
        public int FormationTagId { get; set; }
        public int FormationId { get; set; }
        public string Tag { get; set;}

        public virtual Formation Formation { get; set; }

    }
}
