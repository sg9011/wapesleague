using System;
using System.Collections.Generic;
using WaPesLeague.Data.Entities.User;

namespace WaPesLeague.Data.Entities.SocialMedia
{
    public class SocialMedia
    {
        public int SocialMediaId { get; set; }
        public Guid SocialMediaGuid { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }

        public virtual List<SocialMediaPictureType> PictureTypes { get; set; }
        public virtual List<UserSocialMedia> UserSocialMedias { get; set; }
        //public virtual List<TeamSocialMedia> TeamSocialMedias { get; set; }
        //public virtual List<MatchSocialMedia> MatchSocialMedias { get; set; }
        //public virtual List<FederationSocialMedia> FederationSocialMedias { get; set; }
        //public virtual List<FederationSeasonCompetitionRoundSocialMedia> FederationCompetitionSeasonRoundSocialMedias { get; set; }


    }
}
