using System;

namespace Base.Bot.Queue.Dto
{
    public class MessageCreatedDto
    {
        public ulong DiscordServerId { get; set; }
        public string DiscordServerName { get; set; }
        public DateTime DateTime { get; set; }
        public ulong UserId { get; set; }

        public MessageCreatedDto(ulong discordServerId, string discordServerName, DateTime dateTime, ulong userId)
        {
            DiscordServerId = discordServerId;
            DiscordServerName = discordServerName;
            DateTime = dateTime;
            UserId = userId;
        }
    }
}
