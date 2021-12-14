using System.Collections.Generic;
using WaPesLeague.Data.Entities.Metadata.Enums;
using WaPesLeague.Data.Entities.User;

namespace WaPesLeague.Data.Entities.Metadata
{
    public class Metadata
    {
        public int MetadataId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }

        public PropertyType PropertyType { get; set; }

        public virtual List<UserMetadata> UserMetadatas { get; set; }
    }
}
