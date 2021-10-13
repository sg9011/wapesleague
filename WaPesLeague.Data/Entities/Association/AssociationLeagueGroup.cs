using System;
using System.Collections.Generic;

namespace WaPesLeague.Data.Entities.Association
{
    public class AssociationLeagueGroup
    {
        public int AssociationLeagueGroupId { get; set; }
        public int AssociationId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public DateTime DateCreated { get; set; }

        public virtual Association Association { get; set; }
        public virtual List<AssociationLeagueSeason> AssociationLeagueSeasons { get; set; }

    }
}
