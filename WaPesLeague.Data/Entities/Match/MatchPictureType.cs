using System;
using WaPesLeague.Data.Entities.PictureType;

namespace WaPesLeague.Data.Entities.Match
{
    public class MatchPictureType : EntityPictureType
    {
        public int MatchPictureTypeId { get; set; }
        public int MatchId { get; set; }

        public virtual Match Match { get; set; }

        public override int GetEntityId() => MatchId;
        public override Guid? GetEntityGuid() => Match?.MatchGuid;
    }
}
