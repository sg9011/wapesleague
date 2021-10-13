using System;
using WaPesLeague.Data.Entities.PictureType;

namespace WaPesLeague.Data.Entities.User
{
    public class UserPictureType : EntityPictureType
    {
        public int UserPictureTypeId { get; set; }
        public int UserId { get; set; }

        public virtual User User { get; set; }
        public override Guid? GetEntityGuid() => User?.UserGuid;
        public override int GetEntityId() => UserId;
    }
}
