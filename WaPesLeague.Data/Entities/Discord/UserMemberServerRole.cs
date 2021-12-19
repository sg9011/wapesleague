using WaPesLeague.Data.Entities.User;

namespace WaPesLeague.Data.Entities.Discord
{
    public class UserMemberServerRole
    {
        public int UserMemberServerRoleId { get; set; }
        public int UserMemberId { get; set; }
        public int ServerRoleId { get; set; }

        public virtual ServerRole ServerRole {get; set; }
        public virtual UserMember UserMember { get; set; }
    }
}
