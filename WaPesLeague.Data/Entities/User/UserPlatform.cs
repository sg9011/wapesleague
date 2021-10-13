namespace WaPesLeague.Data.Entities.User
{
    public class UserPlatform
    {
        public int UserPlatformId { get; set; }
        public int UserId { get; set; }
        public int PlatformId { get; set; }
        public string UserName { get; set; }

        public virtual User User { get; set; }
        public virtual Platform.Platform Platform { get; set; }
    }
}
