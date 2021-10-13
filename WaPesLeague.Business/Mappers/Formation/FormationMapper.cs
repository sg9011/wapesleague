using System.Collections.Generic;
using System.Linq;
using WaPesLeague.Data.Entities.Formation;
using WaPesLeague.Data.Entities.Mix;

namespace WaPesLeague.Business.Mappers.Formation
{
    public static class FormationMapper
    {
        public static List<MixChannelTeamPosition> MapFormationToMixChannelTeamPositions(this ServerFormation serverFormation)
        {
            return serverFormation.Positions.Select(fp => new MixChannelTeamPosition() { PositionId = fp.PositionId }).ToList();
        }
    }
}
