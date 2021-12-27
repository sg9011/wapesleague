using System.Collections.Generic;
using WaPesLeague.Data.Entities.Association;

namespace WaPesLeague.Business.Dto.Association
{
    public class UserAndTenantPlayerDto
    {
        public string PlayerName { get; set; }
        public Data.Entities.User.User User { get; set; }
        public List<AppPlayerResultLine> PlayerResultLines { get; set; }
        public AssociationTenantPlayer AssociationTenantPlayer { get; set; }

        public UserAndTenantPlayerDto(string playerName, Data.Entities.User.User user, List<AppPlayerResultLine> playerResultLines, AssociationTenantPlayer associationTenantPlayer)
        {
            PlayerName = playerName;
            User = user;
            PlayerResultLines = playerResultLines;
            AssociationTenantPlayer = associationTenantPlayer;
        }
    }
}
