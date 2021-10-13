using System;

namespace WaPesLeague.Data.Entities.SocialMedia
{
    public abstract class EntitySocialMedia
    {
        public int SocialMediaId { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }

        public virtual SocialMedia SocialMedia{ get; set; }

        public abstract int GetEntityId();
        //This is nullable so the main entity shouldn't be loaded when calling social media.
        public abstract Guid? GetEntityGuid();
    }
}