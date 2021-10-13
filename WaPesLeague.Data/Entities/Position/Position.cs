using System;
using System.Collections.Generic;
using System.Linq;
using WaPesLeague.Data.Entities.Formation;
using WaPesLeague.Data.Entities.Match;
using WaPesLeague.Data.Entities.Mix;

namespace WaPesLeague.Data.Entities.Position
{
    public class Position
    {
        public int PositionId { get; set; }
        public int PositionGroupId { get; set; }
        public int? ParentPositionId { get; set; }
        public int Order { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool IsRequiredForMix { get; set; }

        public virtual PositionGroup PositionGroup { get; set; }
        public virtual Position ParentPosition { get; set; }
        public virtual List<Position> ChildPositions { get; set; }
        public virtual List<MixChannelTeamPosition> MixChannelPositions { get; set; }
        public virtual List<MixPosition> MixPositions { get; set; }
        public virtual List<FormationPosition> FormationPositions { get; set; }
        public virtual List<ServerFormationPosition> ServerFormationPositions { get; set; }
        public virtual List<PositionTag> Tags { get; set; }
        public virtual List<MatchTeamPlayer> MatchTeamPlayers { get; set; }
        public virtual List<MixUserPositionSessionStat> UserSessionStats { get; set; }

        public Position()
        {
            IsRequiredForMix = true;
        }

        public bool HasCodeOrTag(string codeOrTag)
        {
            return string.Equals(codeOrTag, Code, StringComparison.InvariantCultureIgnoreCase)
                || (Tags?.Any(t => string.Equals(codeOrTag, t.Tag, StringComparison.InvariantCultureIgnoreCase)) ?? false);
        }

        public string Display()
        {
            return Tags?.SingleOrDefault(t => t.IsDisplayValue == true)?.Tag ?? Code;
        }
    }
}
