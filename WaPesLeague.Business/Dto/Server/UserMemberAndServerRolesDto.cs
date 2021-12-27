using System.Collections.Generic;
using WaPesLeague.Data.Entities.User;

namespace WaPesLeague.Business.Dto.Server
{
    public class UserMemberAndServerRolesDto
    {
        public UserMember UserMember { get; set; }
        public List<ServerRoleDto> DiscordServerRoleDtos { get; set; }
    }
}
