using System;
using System.Collections.Generic;

namespace WaPesLeague.Data.Entities.ELO
{
    public class EloTeam
    {
        public int EloTeamId { get; set; }
        public string Name { get; set; }
        public string EmojiName { get; set; }
        public string EmojiId { get; set; }
        public string Link { get; set; }
        public DateTime DateCreated { get; set; }
        public int CreatedById { get; set; }

        public virtual User.User CreatedBy { get; set; }
        public virtual List<ELOTeamTag> Tags { get; set; }
    }
}
