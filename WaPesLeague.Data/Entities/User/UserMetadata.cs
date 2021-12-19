using System;
using WaPesLeague.Data.Entities.Metadata;

namespace WaPesLeague.Data.Entities.User
{
    public class UserMetadata : EntityMetadata
    {
        public int UserMetadataId { get; set; }
        public int UserId { get; set; }

        public virtual User User { get; set; }

        public override Guid? GetEntityGuid() => User?.UserGuid;
        public override int GetEntityId() => UserId;
    }
}
