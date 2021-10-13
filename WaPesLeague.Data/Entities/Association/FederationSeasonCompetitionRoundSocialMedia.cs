using System;
using WaPesLeague.Data.Entities.SocialMedia;

namespace WaPesLeague.Data.Entities.Federation
{
    public class FederationSeasonCompetitionRoundSocialMedia : EntitySocialMedia
    {
        public int FederationSeasonCompetitionRoundSocialMediaId { get; set; } 
        public int FederationSeasonCompetitionRoundId { get; set; } 
        public virtual FederationSeasonCompetitionRound FederationSeasonCompetitionRound { get; set; }
        public override Guid? GetEntityGuid() => FederationSeasonCompetitionRound?.FederationSeasonCompetitionRoundGuid;

        public override int GetEntityId() => FederationSeasonCompetitionRoundId;

    }
}
