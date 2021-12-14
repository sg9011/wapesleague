using System;

namespace WaPesLeague.Data.Entities.Metadata
{
    public abstract class EntityMetadata
    {
        public int MetadataId { get; set; }
        public string Value { get; set; }

        public virtual Metadata Metadata { get; set; }

        public abstract int GetEntityId();
        //This is nullable so the main entity shouldn't be loaded when calling social media.
        public abstract Guid? GetEntityGuid();
    }
}
