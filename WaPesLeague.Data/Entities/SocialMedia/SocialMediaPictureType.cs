using System;
using WaPesLeague.Data.Entities.PictureType;

namespace WaPesLeague.Data.Entities.SocialMedia
{
    public class SocialMediaPictureType : EntityPictureType
    {
        public int SocialMediaPictureTypeId { get; set; }
        public int SocialMediaId { get; set; }

        public virtual SocialMedia SocialMedia { get; set; }

        public override int GetEntityId() => SocialMediaId;
        public override Guid? GetEntityGuid() => SocialMedia?.SocialMediaGuid; 
    }
}
