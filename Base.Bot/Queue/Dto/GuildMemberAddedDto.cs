namespace Base.Bot.Queue.Dto
{
    public class GuildMemberAddedDto
    {
        public ulong DiscordServerId { get; set; }
        public string DiscordServerName { get; set; }
        public ulong DiscordUserId { get; set; }

        public GuildMemberAddedDto(ulong discordServerId, string discordServerName,ulong discordUserId)
        {
            DiscordServerId = discordServerId;
            DiscordServerName = discordServerName;
            DiscordUserId = discordUserId;
        }
    }
}
