using System.Collections.Generic;

namespace WaPesLeague.Data.Entities.Formation
{
    public class Formation
    {
        public int FormationId { get; set; }
        public string Name { get; set; }

        public bool IsDefault { get; set; }

        public virtual List<FormationTag> Tags { get; set; }
        public virtual List<FormationPosition> FormationPositions { get; set; }
    }
}
