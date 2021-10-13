using System;
using System.Collections.Generic;

namespace WaPesLeague.Data.Entities.Federation
{
    //WaPes is a Federation
    public class Federation
    {
        public int FederationId { get; set; }
        public Guid FederationGuid { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<FederationSeason> FederationSeasons { get; set; }
    }
}
