using System;
using WaPesLeague.Data.Entities.SocialMedia;

namespace WaPesLeague.Data.Entities.User
{
    public class UserSocialMedia : EntitySocialMedia
    {
        public int UserSocialMediaId { get; set; }
        public int UserId { get; set; }

        public virtual User User { get; set; }

        public override int GetEntityId() => UserId;
        public override Guid? GetEntityGuid() => User?.UserGuid;
    }
}
