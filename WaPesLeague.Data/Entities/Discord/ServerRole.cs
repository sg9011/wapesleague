namespace WaPesLeague.Data.Entities.Discord
{
    public class ServerRole
    {
        public int ServerRoleId { get; set; }
        public int ServerId { get; set; }
        public string DiscordServerRoleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual Server Server { get; set; }
    }
}
