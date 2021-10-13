using WaPesLeague.Data.Entities.Discord;

namespace WaPesLeague.Data.Entities.User
{
    public class UserMember
    {
        public int UserMemberId { get; set; }
        public int UserId { get; set; }
        public int ServerId { get; set; }
        public string DiscordUserName { get; set; }
        public string DiscordNickName { get; set; }
        public string DiscordMention { get; set; }
        public string DiscordUserId { get; set; }

        public virtual User User { get; set; }
        public virtual Server Server { get; set; }
    }
}
