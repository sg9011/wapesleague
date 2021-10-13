using System;
using WaPesLeague.Data.Entities.Federation.Enums;

namespace WaPesLeague.Data.Entities.Federation
{
    public class FederationSeasonRegistration
    {
        public int FederationSeasonRegistrationId { get; set; }
        public Guid SeasonRegistrationGuid { get; set; }
        public int FederationSeasonId { get; set; }
        public int TeamId { get; set; }
        public RegistrationType RegistrationType { get; set; }
        public DateTime DateRequested { get; set; }
        public bool Participate { get; set; }

        public virtual Team.Team Team { get; set; }
        public virtual FederationSeason FederationSeason { get; set; }

    }
}
