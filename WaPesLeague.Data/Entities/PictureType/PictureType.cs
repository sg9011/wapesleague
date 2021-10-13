using System.Collections.Generic;
using WaPesLeague.Data.Entities.SocialMedia;
using WaPesLeague.Data.Entities.User;

namespace WaPesLeague.Data.Entities.PictureType
{
    public class PictureType
    {
        public int PictureTypeId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }

        public virtual List<UserPictureType> UserPictureTypes { get; set; }
        public virtual List<SocialMediaPictureType> SocialMediaPictureTypes { get; set; }
        //public virtual List<MatchPictureType> MatchPictureTypes { get; set; }
        //public virtual List<TeamPictureType> TeamPictureTypes { get; set; }
    }
}
