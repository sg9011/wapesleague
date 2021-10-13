using System;

namespace WaPesLeague.Data.Entities.PictureType
{
    public abstract class EntityPictureType
    {
        public int PictureTypeId { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }

        public virtual PictureType PictureType { get; set; }

        public abstract int GetEntityId();
        //This is nullable so the main entity shouldn't be loaded when calling social media.
        public abstract Guid? GetEntityGuid();
    }
}
