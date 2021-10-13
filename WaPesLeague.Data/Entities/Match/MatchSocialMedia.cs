using System;
using WaPesLeague.Data.Entities.SocialMedia;

namespace WaPesLeague.Data.Entities.Match
{
    public class MatchSocialMedia : EntitySocialMedia
    {
        public int MatchSocialMediaId { get; set; }
        public int MatchId { get; set; }

        public virtual Match Match { get; set; }
        public override int GetEntityId() => MatchId;
        public override Guid? GetEntityGuid() => Match?.MatchGuid;
    }
}
