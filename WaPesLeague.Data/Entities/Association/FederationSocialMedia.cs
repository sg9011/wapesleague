using System;
using WaPesLeague.Data.Entities.SocialMedia;

namespace WaPesLeague.Data.Entities.Federation
{
    public class FederationSocialMedia : EntitySocialMedia
    {
        public int FederationSocialMediaId { get; set; }
        public int FederationId { get; set; }

        public virtual Federation Federation { get; set; }
        public override int GetEntityId() => FederationId;
        public override Guid? GetEntityGuid() => Federation?.FederationGuid;
    }
}
