namespace WaPesLeague.Business.Dto.User
{
    public class PlatformUserDto
    {
        public string PlatformName { get; set; }
        public string UserName { get; set; }
         
        public string ToDiscordString()
        {
            return $"{PlatformName}: {UserName}";
        }
    }
}
